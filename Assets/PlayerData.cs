using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : CardsData
{
    public string[][] decks;
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

    public PlayerData(Dictionary<string, int> myCards, List<string[]> decks, List<string> deckNames) : base(myCards)
    {
        this.decks = decks.ToArray();
        this.deckNames = deckNames.ToArray();
    }
    public PlayerData() : base (new Dictionary<string, int>())
    {

    }
    public List<string[]> getDecks()
    {
        return new List<string[]>(decks);
    }
}
