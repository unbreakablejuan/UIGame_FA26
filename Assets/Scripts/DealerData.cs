using UnityEngine;


public enum GameType { CoinFlip, Uno, Tag}

public class DealerData
{
    public string name;
    public GameType game;
    public int dayProfit;
    public int lifeTimeProfit;
}
