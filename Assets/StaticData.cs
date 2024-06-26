using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public static bool loaded;
    public static PlayerData playerData;
    public static Gameboard board;
    public static List<TradingCard> myCards;
    public static List<int[]> myDecks;

    public static int NUM_CARDS_IN_DECK = 40;
    public static Transform findDeepChild(Transform parent, string childName)
    {
        LinkedList<Transform> kids = new LinkedList<Transform>();
        for (int q = 0; q < parent.childCount; q++)
        {
            kids.AddLast(parent.GetChild(q));
        }
        while (kids.Count > 0)
        {
            Transform current = kids.First.Value;
            kids.RemoveFirst();
            if (current.name == childName || current.name + "(Clone)" == childName)
            {
                return current;
            }
            for (int q = 0; q < current.childCount; q++)
            {
                kids.AddLast(current.GetChild(q));
            }
        }
        return null;
    }
}
