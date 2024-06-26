using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkLogic : NetworkBehaviour
{
    private NetworkVariable<int[]> deckOrder = new NetworkVariable<int[]>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<CardsData> deck = new NetworkVariable<CardsData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private bool done = false;

    public static string GAMEBOARD_NAME = "Gameboard";

    public override void OnNetworkSpawn()
    {
        int[] order = new int[StaticData.NUM_CARDS_IN_DECK];
        for (int q = 0; q < order.Length; q++)
        {
            order[q] = q;
        }
        for (int q = 0; q < order.Length; q++)
        {
            int replaceLoc = Random.Range(0, order.Length);
            int temp = order[q];
            order[q] = order[replaceLoc];
            order[replaceLoc] = temp;
        }
        deckOrder.Value = order;

        deck.Value
            = GameObject.Find(GAMEBOARD_NAME).GetComponent<Gameboard>().getMyDeckData();
    }

    // Update is called once per frame
    void Update()
    {
        if (!done && !IsOwner && deckOrder.Value != null && deck.Value != null)
        {
            GameObject.Find(GAMEBOARD_NAME)
                .GetComponent<Gameboard>().setDecks(deck.Value, deckOrder.Value);
            GameObject.Find(GAMEBOARD_NAME)
                .GetComponent<Gameboard>().decideFirstPlayer(IsServer);
            done = true;
        }
    }

    public struct CardsSendData : INetworkSerializable
    {

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {

        }
    }
}
