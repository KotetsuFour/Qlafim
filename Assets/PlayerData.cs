using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : CardsData
{
    public int[][] decks;
    public string[] deckNames;
    public int selectedDeck;

    public string playerName;
    public byte[] profileImage;

    public bool initialized;

    public int cardsPlayed;
    public int humansPlayed;
    public int nonHumansPlayed;
    public int cardsDestroyed;
    public int cardsBanished;
    public int cardsRevived;
    public int cardsRedeemed;

    public PlayerData(List<TradingCard> cards, List<int[]> decks) : base(cards)
    {
        this.decks = new int[decks.Count][];
        for (int q = 0; q < decks.Count; q++)
        {
            this.decks[q] = decks[q];
        }
    }
    public PlayerData()
    {

    }
    public List<int[]> getAllDecks()
    {
        return new List<int[]>(decks);
    }

    public void addDeck(int[] deck, string deckName)
    {
        if (decks == null)
        {
            decks = new int[][] { deck };
            deckNames = new string[] { deckName };
            return;
        }
        List<int[]> myDecksAdd = getAllDecks();
        myDecksAdd.Add(deck);
        decks = myDecksAdd.ToArray();

        List<string> myDeckNamesAdd = new List<string>(deckNames);
        myDeckNamesAdd.Add(deckName);
        deckNames = myDeckNamesAdd.ToArray();
    }
}
