using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private void Start()
    {
        SaveCheckpoint();
    }

    public void SaveCheckpoint()
    {
        // Find all game objects tagged as "Burger Piece"
        GameObject[] burgerPieces = GameObject.FindGameObjectsWithTag("Burger Piece");

        // Store count for potential future use
        PlayerPrefs.SetInt("Checkpoint_BurgerPiece_Count", burgerPieces.Length);

        // Save each transform position
        for (int i = 0; i < burgerPieces.Length; i++)
        {
            Transform t = burgerPieces[i].transform;
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_x", t.position.x);
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_y", t.position.y);
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_z", t.position.z);
        }

        // Ensure data is written to disk
        PlayerPrefs.Save();
    }
}
