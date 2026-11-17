using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ClosedController : MonoBehaviour
{
    public TMP_Text coinFlipLabel;
    public TMP_Text unoLabel;
    public TMP_Text tagLabel;
    public TMP_Text moneyLabel;
    public TMP_Text dayEarningsLabel;
    public TMP_Text lifetimeEarningsLabel;
    public int coinFlipPrice = 150;
    public int unoPrice = 400;
    public int tagPrice = 600;
    public GameObject cardPrefab;
    public Transform cardGrid;
    public TMP_Text wagesLabel;
   




    string[] dealerNames = { "Dutch", "Lenny", "Javiar", "Aruther", "Hosea", "Michah", "Sadie", "Charles", "John", "Abigail", "Jack", "Josiah", "Mary-Beth", "Pearson", "Tilly" };

    void Start()
    {
        UpdateLabels();
        ShowNumbers();
        foreach (DealerData d in GameState.dealers) SpawnCard(d); // rebuild cards for dealers already hired
    }

    void OnValidate()
    {
        UpdateLabels();
    }

    public void Hire(int game)
    {
        int[] prices = { coinFlipPrice, unoPrice, tagPrice };
        int cost = prices[game];
        if (GameState.money < cost) { Debug.Log("Not enough money to hire dealer for game " + game); return; }
        if (GameState.dealers.Count >= 6) { Debug.Log("Not enough room for more dealers"); return; }

        GameState.money -= cost;
        DealerData dealerData = new DealerData();
        dealerData.name = dealerNames[Random.Range(0, dealerNames.Length)];
        dealerData.game = (GameType)game;
        GameState.dealers.Add(dealerData);
        Debug.Log("Hired dealer " + dealerData.name + " for game " + dealerData.game + ". Remaining money: " + GameState.money);

        SpawnCard(dealerData);
        ShowNumbers();
    }

    void SpawnCard(DealerData d)
    {
        GameObject card = Instantiate(cardPrefab, cardGrid);
        card.transform.Find("DealersName").GetComponent<TMP_Text>().text = d.name;
        card.transform.Find("DaysWinningsNumber").GetComponent<TMP_Text>().text = "$" + d.dayProfit;
        card.transform.Find("LifeTimeNumber").GetComponent<TMP_Text>().text = "$" + d.lifeTimeProfit;

        Button fire = card.transform.Find("Fire").GetComponent<Button>();
        fire.onClick.AddListener(() => FireDealer(d, card));
    }

    void FireDealer(DealerData d, GameObject card)
    {
        GameState.dealers.Remove(d);
        Destroy(card);
        ShowNumbers();
    }

    void ShowNumbers() // updates the money, day earnings, lifetime earnings, and wages labels
    {
        if (moneyLabel != null) moneyLabel.text = "$" + GameState.money;
        if (dayEarningsLabel != null) dayEarningsLabel.text = "$" + GameState.dayEarnings;
        if (lifetimeEarningsLabel != null) lifetimeEarningsLabel.text = "$" + GameState.lifetimeEarnings;
        if (wagesLabel != null) wagesLabel.text = "$" + GameState.WagesPerDay();
    }

    void UpdateLabels() // updates the labels for the cost of hiring dealers for each game
    {
        if (coinFlipLabel != null) coinFlipLabel.text = "Coin Flip\n$" + coinFlipPrice + "/day";
        if (unoLabel != null) unoLabel.text = "UNO\n$" + unoPrice + "/day";
        if (tagLabel != null) tagLabel.text = "Tag\n$" + tagPrice + "/day";
    }

    public void StartDay()
    {
        SceneManager.LoadScene("CasinoOpen");
    }
}