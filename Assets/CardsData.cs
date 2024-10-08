using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardsData
{
    public string[] cardIDs;
    public int[] cardAmounts;

    public CardsData(Dictionary<string, int> myCards)
    {
        initialize(myCards.Count);

        int idx = 0;
        Dictionary<string, int>.KeyCollection keys = myCards.Keys;
        foreach (string id in keys)
        {
            cardIDs[idx] = id;
            cardAmounts[idx] = myCards[id];
            idx++;
        }
    }
    public void initialize(int amount)
    {
        cardIDs = new string[amount];
        cardAmounts = new int[amount];
    }

    public Dictionary<string, int> getCards()
    {
        Dictionary<string, int> ret = new Dictionary<string, int>();
        for (int q = 0; q < cardIDs.Length; q++)
        {
            ret.Add(cardIDs[q], cardAmounts[q]);
        }
        return ret;
    }
}
