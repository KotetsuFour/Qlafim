using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : PositionState
{
    public new void updatePositions()
    {
        int border = (positions.Count - cardsHere.Count) / 2;
        for (int q = 0; q < cardsHere.Count; q++)
        {
            cardsHere[q].transform.position = positions[(q + border) % positions.Count].position;
            cardsHere[q].transform.rotation = positions[(q + border) % positions.Count].rotation;
        }
    }
}
