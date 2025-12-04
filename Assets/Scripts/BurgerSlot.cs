using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BurgerSlot : MonoBehaviour, ISnappable
{
    [Tooltip("Index in the stack: 0 = bottom, increasing upwards.")]
    public int slotIndex = 0;

    [Tooltip("Expected pieceId for this slot. Leave empty to accept any piece.")]
    public string expectedPieceId = "";

    [Tooltip("Local transform used as the exact snap position/rotation.")]
    public Transform snapAnchor;

    private GameObject anchor;

    [HideInInspector]
    public BurgerPiece acceptedPiece;

    public UnityEvent onPiecePlaced;
    public UnityEvent onPieceRemoved;

    public string burgerID;
    bool IsSnapped = false;

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

    
    

    public void Clear()
    {
        //if (acceptedPiece != null)
        //{
        //    onPieceRemoved?.Invoke();
        //    acceptedPiece = null;
        //    BurgerAssemblyManager.Instance?.RegisterRemoval(this);
        //}
    }

    public void SnapToSlot(BurgerPiece ID, string burgerID)
    {
        if(IsSnapped) return;
        IsSnapped = true;
        var obj = ID.transform.gameObject;
        this.burgerID = burgerID;

        obj.transform.position = transform.position;
        if (this.anchor == null)
        {
            var anchor = new GameObject("Anchor");
            this.anchor = anchor;
        }
        anchor.transform.SetParent(transform);
        obj.transform.SetParent(anchor.transform);
    }

    public void ReleaseFromSlot()
    {
        if(!IsSnapped) return;
        IsSnapped = false;
        burgerID = null;

    }
}
