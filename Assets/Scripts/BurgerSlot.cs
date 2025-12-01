using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BurgerSlot : MonoBehaviour
{
    [Tooltip("Index in the stack: 0 = bottom, increasing upwards.")]
    public int slotIndex = 0;

    [Tooltip("Expected pieceId for this slot. Leave empty to accept any piece.")]
    public string expectedPieceId = "";

    [Tooltip("Local transform used as the exact snap position/rotation.")]
    public Transform snapAnchor;

    [HideInInspector]
    public BurgerPiece acceptedPiece;

    public UnityEvent onPiecePlaced;
    public UnityEvent onPieceRemoved;

    void Reset()
    {
        // ensure collider is trigger so pieces can pass through while being moved
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    public Vector3 GetSnapPosition()
    {
        return (snapAnchor != null) ? snapAnchor.position : transform.position;
    }

    public Quaternion GetSnapRotation()
    {
        return (snapAnchor != null) ? snapAnchor.rotation : transform.rotation;
    }

    // Try to accept a piece into this slot. Returns true if accepted.
    public bool TryPlace(BurgerPiece piece)
    {
        if (piece == null) return false;
        if (acceptedPiece != null) return false; // occupied
        if (!string.IsNullOrEmpty(expectedPieceId) && piece.pieceId != expectedPieceId) return false;

        acceptedPiece = piece;
        piece.SnapToSlot(this);
        onPiecePlaced?.Invoke();
        BurgerAssemblyManager.Instance?.RegisterPlacement(this);
        return true;
    }

    public void Clear()
    {
        if (acceptedPiece != null)
        {
            onPieceRemoved?.Invoke();
            acceptedPiece = null;
            BurgerAssemblyManager.Instance?.RegisterRemoval(this);
        }
    }
}
