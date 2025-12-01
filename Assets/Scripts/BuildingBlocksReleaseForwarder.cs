using UnityEngine;

public class BuildingBlocksReleaseForwarder : MonoBehaviour
{
    public BurgerPiece piece;

    // Call this from the BuildingBlocks release UnityEvent (or inspector)
    public void OnReleased()
    {
        if (piece == null) piece = GetComponent<BurgerPiece>();
        piece?.OnReleasedByPlayer();
    }
}