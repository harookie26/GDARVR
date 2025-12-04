using UnityEngine;

public interface ISnappable
{
    public void SnapToSlot(BurgerPiece ID, string burgerID);

    public void ReleaseFromSlot();
}
