using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BurgerAssemblyManager : MonoBehaviour
{
    public static BurgerAssemblyManager Instance { get; private set; }

    [Tooltip("Ordered list of slots from bottom (index 0) to top.")]
    public List<BurgerSlot> slots = new List<BurgerSlot>();

    [Tooltip("Recipe that defines expected piece IDs per slot.")]
    public BurgerRecipe recipe;

    [Tooltip("Maximum distance to auto-snap on release (meters).")]
    public float autoSnapRadius = 0.12f;

    [Tooltip("Event invoked when the burger is correctly assembled.")]
    public UnityEvent onBurgerCompleted;

    bool completed = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        // Basic sanity: if we have a recipe, ensure slots length matches (warn only)
        if (recipe != null && recipe.pieceIds.Count != slots.Count)
            Debug.LogWarning($"Recipe pieces ({recipe.pieceIds.Count}) != slots ({slots.Count}).");
    }

    // Called by slot when a piece is placed
    public void RegisterPlacement(BurgerSlot slot)
    {
        if (completed) return;
        ValidateAssembly();
    }

    // Called by slot when a piece is removed
    public void RegisterRemoval(BurgerSlot slot)
    {
        if (completed) return;
    }

    // Finds nearest available slot within radius that either expects this piece or accepts any
    public BurgerSlot FindNearestSlot(Vector3 worldPosition, float radius)
    {
        BurgerSlot closest = null;
        float best = radius;
        foreach (var s in slots)
        {
            if (s.acceptedPiece != null) continue;
            float d = Vector3.Distance(worldPosition, s.GetSnapPosition());
            if (d <= best)
            {
                best = d;
                closest = s;
            }
        }
        return closest;
    }

    void ValidateAssembly()
    {
        // Quick check: all slots filled
        if (slots.Any(s => s.acceptedPiece == null)) return;

        if (recipe != null && recipe.pieceIds.Count == slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var expected = recipe.pieceIds[i];
                var actual = slots[i].acceptedPiece?.pieceId;
                if (expected != actual)
                {
                    // incorrect order or wrong piece
                    return;
                }
            }
        }

        // All checks passed
        CompleteBurger();
    }

    void CompleteBurger()
    {
        completed = true;

        // Make entire assembled burger a single kinematic group
        var root = new GameObject("AssembledBurger");
        root.transform.position = slots[0].transform.position;
        for (int i = 0; i < slots.Count; i++)
        {
            var piece = slots[i].acceptedPiece;
            if (piece == null) continue;

            // parent under root and make kinematic
            piece.transform.SetParent(root.transform, true);
            var rb = piece.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            var col = piece.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        onBurgerCompleted?.Invoke();
    }

    // Optional helper to forcibly assemble regardless of recipe (useful for testing)
    public void ForceAssemble()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].acceptedPiece != null) continue;
        }
        CompleteBurger();
    }

    // Reset manager for replays (not destroying pieces)
    public void ResetAssembly()
    {
        completed = false;
        foreach (var s in slots)
        {
            if (s.acceptedPiece != null)
            {
                s.acceptedPiece.ReleaseFromSlot();
                s.Clear();
            }
        }
    }
}
