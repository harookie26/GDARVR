using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BurgerPiece : MonoBehaviour
{
    [Tooltip("Unique ID that matches recipe and slots (e.g. \"BottomBun\", \"Patty\", \"Cheese\").")]
    public string pieceId;

    [Tooltip("Optional visual anchor used for snapping (if left null, the piece's root transform is used).")]
    public Transform snapAnchor;

    Rigidbody rb;
    Collider coll;
    public bool IsPlaced { get; private set; } = false;
    public BurgerSlot CurrentSlot { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        if (snapAnchor == null) snapAnchor = transform;
    }

    // Called by a slot to lock the piece in place
    public void SnapToSlot(BurgerSlot slot)
    {
        if (slot == null) return;

        IsPlaced = true;
        CurrentSlot = slot;

        // Parent and match anchor
        transform.SetParent(slot.transform, true);
        transform.position = slot.GetSnapPosition();
        transform.rotation = slot.GetSnapRotation();

        // Stop physics while placed
        if (rb != null) rb.isKinematic = true;
        if (coll != null) coll.isTrigger = true;
    }

    // Called when the player grabs the piece (or when we want to un-place it)
    public void ReleaseFromSlot()
    {
        if (!IsPlaced) return;

        // Unlink
        transform.SetParent(null, true);

        if (rb != null)
        {
            rb.isKinematic = false;
            // optional: zero velocities so it doesn't fling
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (coll != null) coll.isTrigger = false;

        IsPlaced = false;
        if (CurrentSlot != null) CurrentSlot.Clear();
        CurrentSlot = null;
    }

    // Helper to be invoked from your grab system on release.
    // Example: XRGrabInteractable.onSelectExited -> call this, then Ask manager to attempt placement.
    public void OnReleasedByPlayer()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherPiece = other.GetComponent<ISnappable>();

        if(otherPiece != null && IsPlaced == false)
        {
            var ID = GetComponent<BurgerPieceID>();
            var burgerID = ID.GetPieceID();
            otherPiece.SnapToSlot(this, burgerID);
            IsPlaced = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherPiece = other.GetComponent<ISnappable>();

        if (otherPiece != null && IsPlaced == true)
        {
            otherPiece.ReleaseFromSlot();
            transform.SetParent(null);
            IsPlaced = false;
        }
    }
}
