using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : PositionState
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void shuffle()
    {
        if (cardsHere.Count <= 0)
        {
            return;
        }
        for (int q = 0; q < cardsHere.Count; q++)
        {
            int randIdx = Random.Range(0, cardsHere.Count);
            TradingCard temp = cardsHere[randIdx];
            cardsHere[randIdx] = cardsHere[q];
            cardsHere[q] = temp;
        }
        updatePositions();
    }

}
