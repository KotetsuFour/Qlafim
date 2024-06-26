using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionState : MonoBehaviour
{
    public List<TradingCard> cardsHere;
    public List<Transform> positions;
    [SerializeField] private bool autoRotate;
    [SerializeField] private int maxCards;
    public void addCard(TradingCard card)
    {
        cardsHere.Add(card);
        card.positionState = this;
        updatePositions();
    }
    public void insertCard(int idx, TradingCard card)
    {
        cardsHere.Insert(idx, card);
        card.positionState = this;
        updatePositions();
    }
    public void remove(TradingCard card)
    {
        cardsHere.Remove(card);
        updatePositions();
    }
    public TradingCard remove(int idx)
    {
        TradingCard ret = cardsHere[idx];
        cardsHere.RemoveAt(idx);
        updatePositions();
        return ret;
    }
    public TradingCard get(int idx)
    {
        return cardsHere[idx];
    }
    public void updatePositions()
    {
        for (int q = 0; q < cardsHere.Count; q++)
        {
            Transform dest = positions[q % positions.Count];
            cardsHere[q].transform.position = dest.position;
            if (autoRotate)
            {
                cardsHere[q].transform.rotation = dest.rotation;
            }
        }
    }
    public bool isFull()
    {
        return cardsHere.Count >= maxCards;
    }
}
