using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BurgerAssemblyManager : MonoBehaviour
{
    public static BurgerAssemblyManager Instance { get; private set; }

    [Tooltip("Recipe that defines expected piece IDs per slot.")]
    public BurgerRecipe recipe;

    [Tooltip("Defines the desired slot order (indices bottom-to-top).")]
    public BurgerSlotOrder slotOrder;

    public List<BurgerSet> burgerSet;

    private BurgerSet activeSet;

    [Tooltip("Event invoked when the burger is correctly assembled.")]
    public UnityEvent onBurgerCompleted;

    public GameObject CompletedText;

    [SerializeField] private GameObject gameCompleteText;

    public bool completed = false;

    public int score = 0; 
    
    public TMPro.TextMeshProUGUI scoreText;

    private List<BurgerSlot> orderedSlots = new List<BurgerSlot>();

    private CheckpointManager cp => FindFirstObjectByType<CheckpointManager>();

    private Timer gameTimer => FindFirstObjectByType<Timer>();

    private bool TimerRunning = false;
    private bool gameOver = false;

    [SerializeField] private GameObject burgerModel1;
    [SerializeField] private GameObject burgerModel2;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        BuildOrderedSlotsFromOrderAsset();

        activeSet = burgerSet[0];

        recipe = activeSet.recipe;
        slotOrder = activeSet.slotOrder;

        if (recipe != null && recipe.pieceIds.Count != orderedSlots.Count)
            Debug.LogWarning($"Recipe pieces ({recipe.pieceIds.Count}) != slots ({orderedSlots.Count}).");

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }

        CompletedText.SetActive(false);

    }

    private void BuildOrderedSlotsFromOrderAsset()
    {
        var allSlots = FindObjectsOfType<BurgerSlot>(includeInactive: true);
        var byIndex = allSlots.GroupBy(s => s.slotIndex).ToDictionary(g => g.Key, g => g.First());

        orderedSlots.Clear();

        if (slotOrder == null || slotOrder.slotIndex == null || slotOrder.slotIndex.Count == 0)
        {
            Debug.LogWarning("BurgerSlotOrder is not assigned or empty. Falling back to sorting by slotIndex found in scene.");
            orderedSlots = allSlots.OrderBy(s => s.slotIndex).ToList();
            return;
        }

        foreach (var idx in slotOrder.slotIndex)
        {
            if (byIndex.TryGetValue(idx, out var slot))
            {
                orderedSlots.Add(slot);
            }
            else
            {
                Debug.LogWarning($"BurgerSlotOrder references slotIndex {idx} but no BurgerSlot with that index exists in the scene.");
            }
        }
    }

    public void ValidateAssembly()
    {

        if (recipe != null && recipe.pieceIds.Count == orderedSlots.Count)
        {
            for (int i = 0; i < orderedSlots.Count; i++)
            {
                var expected = recipe.pieceIds[i];
                var actual = orderedSlots[i].burgerID;
                if (expected != actual)
                {
                    // incorrect order or wrong piece
                    return;
                }
            }
        }

        StartCoroutine(CompleteBurgerCoroutine());
    }

    private IEnumerator CompleteBurgerCoroutine()
    {
        completed = true;

        onBurgerCompleted?.Invoke();

        if (CompletedText != null) CompletedText.SetActive(true);

        score++;

        if (scoreText != null) scoreText.text = "Score: " + score.ToString();

        yield return new WaitForSeconds(2f);

        if (CompletedText != null) CompletedText.SetActive(false);

        completed = false;
        cp.LoadCheckpoint();
        RandomizeBurgerSet();
    }

    public void ResetAssembly()
    {
        BuildOrderedSlotsFromOrderAsset();
        completed = false;
    }

    private void RandomizeBurgerSet()
    {
        activeSet = burgerSet[Random.Range(0, burgerSet.Count - 1)];
    }

    private void SetActiveBurgerModel()
    {
        int index = burgerSet.IndexOf(activeSet);

        switch (index)
        {
            case 0:
                burgerModel1.SetActive(true);
                burgerModel2.SetActive(false);
                break;
            case 1:
                burgerModel1.SetActive(false);
                burgerModel2.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void Update()
    {

        TimerRunning = gameTimer.GetTimerRunning();
        gameOver = gameTimer.GetGameOver();

        if (!TimerRunning) Time.timeScale = 0.00000001f;

        if (gameOver)
        {
            Time.timeScale = 0.000001f;
            CompletedText.SetActive(true);
        }

        SetActiveBurgerModel();
    }
}
