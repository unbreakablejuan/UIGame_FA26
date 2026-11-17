using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class OpenController : MonoBehaviour
{
    bool dealing;
    public float dayLength = 30f;
    public Image timeLeft;
    float timer;
    public float coinFlipWinChance = 0.5f;
    public float customerWaiting = 40;
    public TMP_Text customersWaitingLabel;
    public TMP_Text coinFlipWagerLabel;
    public TMP_Text coinFlipResultLabel;
    public TMP_Text daysEarningsLabel;
    public float coinFlipTime = 5f;
    public float unoTime = 5f;
    public float tagTime = 5f;
    int handsInProgress;
    bool loading;
    public GameObject cardPrefab;
    public Transform cardGrid;
    Dictionary<DealerData, GameObject> cards = new Dictionary<DealerData, GameObject>(); // dictionary remembers which card belongs to which dealer, so a result can update the right card.


    float TimeFor(GameType g)
        {
            if (g == GameType.Uno) return unoTime;
            if (g == GameType.Tag) return tagTime;
            return coinFlipTime;
        }
    void Start()
    {
        timer = dayLength;
        GameState.dayEarnings = 0; // reset daily earnings at the start of the day
        foreach (DealerData d in GameState.dealers) // start the dealer routines for each hired dealer
        {
            d.dayProfit = 0;
            SpawnCard(d);
            StartCoroutine(DealerRoutine(d));
        }
        ShowDaysEarnings(); // update the days earnings label at the start of the day

    }

   void Update()
    {
        if (loading) return; // if the scene is already loading, do not update the timer
        timer -= Time.deltaTime; // creates a timer
        float t = Mathf.Clamp01(timer / dayLength); // 
        timeLeft.fillAmount = t; // handles the bar length
        if (timer <= 0f && !dealing && handsInProgress == 0) // changes scene when time runs out
            {
                loading = true;
                SceneManager.LoadScene("CasinoClosed");
            }
    }

    int PickWager()
    {
        float r = Random.value;
        if (r < 0.6f) return Random.Range(10, 31);       // the poors
        else if (r < 0.9f) return Random.Range(31, 101); // the middle class
        else return Random.Range(101, 301);              // the rich
    }

    public void DealCoinFlip()
    {
        if (dealing || customerWaiting <= 0) return; // if already dealing or no customers waiting, do nothing
        StartCoroutine(CoinFlipRoutine());
    }

    IEnumerator CoinFlipRoutine() // handles the coin flip game logic
    {
        dealing = true;
        customerWaiting--;
        int wager = PickWager();
        coinFlipWagerLabel.text = "$" + wager;
        coinFlipResultLabel.text = "...";
        customersWaitingLabel.text = customerWaiting.ToString();

        yield return new WaitForSeconds(coinFlipTime);

        bool customerWins = Random.value < coinFlipWinChance;
        int result = customerWins ? -wager : wager; // from the house's side
        coinFlipResultLabel.text = customerWins ? "Loss" : "Win";

        GameState.money += result;
        GameState.dayEarnings += result;
        GameState.lifetimeEarnings += result;
        ShowDaysEarnings();
        Debug.Log("Customer wagered $" + wager + " and " + (customerWins ? "won" : "lost") + ". Result: $" + result);
        dealing = false;
    }
    void ShowDaysEarnings()
    {
        int e = GameState.dayEarnings;
        daysEarningsLabel.text = (e >= 0 ? "+$" : "-$") + Mathf.Abs(e);
    }

    void SpawnCard(DealerData d)
    {
        GameObject card = Instantiate(cardPrefab, cardGrid);
        card.transform.Find("Fire").gameObject.SetActive(false); // no firing in the middle of a day
        cards[d] = card;
        RefreshCard(d);
    }

void RefreshCard(DealerData d)
    {
        GameObject card = cards[d];
        card.transform.Find("DealersName").GetComponent<TMP_Text>().text = d.name;
        card.transform.Find("DaysWinningsNumber").GetComponent<TMP_Text>().text = "$" + d.dayProfit;
        card.transform.Find("LifeTimeNumber").GetComponent<TMP_Text>().text = "$" + d.lifeTimeProfit;
    }


IEnumerator DealerRoutine(DealerData d) // handles the game logic for a dealer, dealing hands until the day ends or there are no more customers waiting
    {
        while (timer > 0f && customerWaiting > 0)
        {
            customerWaiting--;
            handsInProgress++;
            customersWaitingLabel.text = customerWaiting.ToString();
            int wager = PickWager();

            yield return new WaitForSeconds(TimeFor(d.game));

            bool customerWins = Random.value < coinFlipWinChance;
            int result = customerWins ? -wager : wager;
            GameState.money += result;
            GameState.dayEarnings += result;
            GameState.lifetimeEarnings += result;
            d.dayProfit += result;
            d.lifeTimeProfit += result;
            RefreshCard(d);
            handsInProgress--;
            ShowDaysEarnings();
            Debug.Log(d.name + " dealt: " + result);
        }
    }
}