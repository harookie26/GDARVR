using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BurgerAssemblyManager : MonoBehaviour
{
    public static BurgerAssemblyManager Instance { get; private set; }

    [Tooltip("Ordered list of slots from bottom (index 0) to top.")]
    public List<BurgerSlot> slots = new List<BurgerSlot>();

    [Tooltip("Recipe that defines expected piece IDs per slot.")]
    public BurgerRecipe recipe;

    

    [Tooltip("Event invoked when the burger is correctly assembled.")]
    public UnityEvent onBurgerCompleted;

    public GameObject CompletedText;

    bool completed = false;

    public int score = 0; 
    
    public TMPro.TextMeshProUGUI scoreText;

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

    

    

    void ValidateAssembly()
    {

        if (recipe != null && recipe.pieceIds.Count == slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                var expected = recipe.pieceIds[i];
                var actual = slots[i].burgerID;
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


        onBurgerCompleted?.Invoke();
        if (CompletedText != null)
        {
            CompletedText.SetActive(true);
        }
        score++;

    }

   

    // Reset manager for replays (not destroying pieces)
    public void ResetAssembly()
    {
        
        
    }


    private void Update()
    {
        scoreText.text = "Score: " + score.ToString();
        ValidateAssembly();
    }
}
