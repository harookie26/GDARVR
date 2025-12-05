using UnityEngine;
using UnityEngine.UIElements;

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

            Vector3 rot = burgerPieces[i].transform.eulerAngles;
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_rot_x", rot.x);
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_rot_y", rot.y);
            PlayerPrefs.SetFloat($"Checkpoint_BurgerPiece_{i}_rot_z", rot.z);
        }

        // Ensure data is written to disk
        PlayerPrefs.Save();
    }

    public void LoadCheckpoint()
    {
        // Retrieve the count of burger pieces
        int count = PlayerPrefs.GetInt("Checkpoint_BurgerPiece_Count", 0);
        // Load each transform position
        for (int i = 0; i < count; i++)
        {
            float x = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_x", 0f);
            float y = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_y", 0f);
            float z = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_z", 0f);

            float rot_x = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_rot_x", 0f);
            float rot_y = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_rot_y", 0f);
            float rot_z = PlayerPrefs.GetFloat($"Checkpoint_BurgerPiece_{i}_rot_z", 0f);

            // Find the corresponding burger piece
            GameObject burgerPiece = GameObject.FindGameObjectsWithTag("Burger Piece")[i];
            if (burgerPiece != null)
            {
                burgerPiece.transform.parent = null; // Detach from any parent
                burgerPiece.transform.eulerAngles = new Vector3(rot_x, rot_y, rot_z);
                burgerPiece.transform.position = new Vector3(x, y, z);
            }
        }
    }
}
