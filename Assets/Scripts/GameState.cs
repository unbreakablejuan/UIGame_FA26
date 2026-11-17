using UnityEngine;
using System.Collections.Generic;

public static class GameState
{
    public static float money = 1000f;
    public static int day = 1;
    public static List<DealerData> dealers = new List<DealerData>();
    public static int dayEarnings = 0;
    public static int lifetimeEarnings = 0;

    public static int coinFlipWage = 10;
    public static int unoWage = 25;
    public static int tagWage = 40;

    public static int WagesPerDay()
    {
        int total = 0;
        foreach (DealerData d in dealers)
        {
            if (d.game == GameType.CoinFlip) total += coinFlipWage;
            else if (d.game == GameType.Uno) total += unoWage;
            else total += tagWage;
        }
        return total;
    }
}