using TMPro;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    public int currency = 0;
    public int health = 100;
    public int currentRound = 1;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI roundText;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText.text = $"{health}";
        currencyText.text = $"{currency}";
        roundText.text = $"Round: {currentRound}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageTower(int damage)
    {
        health -= damage;
        healthText.text = $"{health}";
    }

    public void AddCurrency(int totale)
    {
        currency += totale;
        currencyText.text = $"{currency}";
    }

    public void AddRound(int totale)
    {
        currentRound += totale;
        roundText.text = $"Round: {currentRound}";
    }
}
