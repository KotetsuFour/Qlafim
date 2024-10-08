using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Unity.Networking;

public class NetworkLogic : NetworkBehaviour
{
    private NetworkVariable<DeckSendData> serverDeck = new NetworkVariable<DeckSendData>(new DeckSendData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<PlayerAction> serverPlayerAction = new NetworkVariable<PlayerAction>(new PlayerAction(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<DeckSendData> clientDeck = new NetworkVariable<DeckSendData>(new DeckSendData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<PlayerAction> clientPlayerAction = new NetworkVariable<PlayerAction>(new PlayerAction(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private bool done = false;
    private int actionsReceivedFromServer;
    private int actionsReceivedFromClient;
    private int actionsSentFromServer;
    private int actionsSentFromClient;

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
        GameObject.Find(GAMEBOARD_NAME).GetComponent<Gameboard>().setMyDeck(order);

        DeckSendData deckData = new DeckSendData();
        deckData.filled = true;
        deckData.card0 = StaticData.myDecks[StaticData.deckInUse][order[0]];
        deckData.card1 = StaticData.myDecks[StaticData.deckInUse][order[1]];
        deckData.card2 = StaticData.myDecks[StaticData.deckInUse][order[2]];
        deckData.card3 = StaticData.myDecks[StaticData.deckInUse][order[3]];
        deckData.card4 = StaticData.myDecks[StaticData.deckInUse][order[4]];
        deckData.card5 = StaticData.myDecks[StaticData.deckInUse][order[5]];
        deckData.card6 = StaticData.myDecks[StaticData.deckInUse][order[6]];
        deckData.card7 = StaticData.myDecks[StaticData.deckInUse][order[7]];
        deckData.card8 = StaticData.myDecks[StaticData.deckInUse][order[8]];
        deckData.card9 = StaticData.myDecks[StaticData.deckInUse][order[9]];
        deckData.card10 = StaticData.myDecks[StaticData.deckInUse][order[10]];
        deckData.card11 = StaticData.myDecks[StaticData.deckInUse][order[11]];
        deckData.card12 = StaticData.myDecks[StaticData.deckInUse][order[12]];
        deckData.card13 = StaticData.myDecks[StaticData.deckInUse][order[13]];
        deckData.card14 = StaticData.myDecks[StaticData.deckInUse][order[14]];
        deckData.card15 = StaticData.myDecks[StaticData.deckInUse][order[15]];
        deckData.card16 = StaticData.myDecks[StaticData.deckInUse][order[16]];
        deckData.card17 = StaticData.myDecks[StaticData.deckInUse][order[17]];
        deckData.card18 = StaticData.myDecks[StaticData.deckInUse][order[18]];
        deckData.card19 = StaticData.myDecks[StaticData.deckInUse][order[19]];
        deckData.card20 = StaticData.myDecks[StaticData.deckInUse][order[20]];
        deckData.card21 = StaticData.myDecks[StaticData.deckInUse][order[21]];
        deckData.card22 = StaticData.myDecks[StaticData.deckInUse][order[22]];
        deckData.card23 = StaticData.myDecks[StaticData.deckInUse][order[23]];
        deckData.card24 = StaticData.myDecks[StaticData.deckInUse][order[24]];
        deckData.card25 = StaticData.myDecks[StaticData.deckInUse][order[25]];
        deckData.card26 = StaticData.myDecks[StaticData.deckInUse][order[26]];
        deckData.card27 = StaticData.myDecks[StaticData.deckInUse][order[27]];
        deckData.card28 = StaticData.myDecks[StaticData.deckInUse][order[28]];
        deckData.card29 = StaticData.myDecks[StaticData.deckInUse][order[29]];
        deckData.card30 = StaticData.myDecks[StaticData.deckInUse][order[30]];
        deckData.card31 = StaticData.myDecks[StaticData.deckInUse][order[31]];
        deckData.card32 = StaticData.myDecks[StaticData.deckInUse][order[32]];
        deckData.card33 = StaticData.myDecks[StaticData.deckInUse][order[33]];
        deckData.card34 = StaticData.myDecks[StaticData.deckInUse][order[34]];
        deckData.card35 = StaticData.myDecks[StaticData.deckInUse][order[35]];
        deckData.card36 = StaticData.myDecks[StaticData.deckInUse][order[36]];
        deckData.card37 = StaticData.myDecks[StaticData.deckInUse][order[37]];
        deckData.card38 = StaticData.myDecks[StaticData.deckInUse][order[38]];
        deckData.card39 = StaticData.myDecks[StaticData.deckInUse][order[39]];

        if (IsServer)
        {
            Debug.Log("set host deck");
            serverDeck.Value = deckData;
        }
        else
        {
            Debug.Log("set client deck");
            DeckServerRpc(deckData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            /*
            Debug.Log("clientDeck filled: " + clientDeck.Value.filled);
            Debug.Log("serverDeck filled: " + serverDeck.Value.filled);
            */
            if (IsServer && clientDeck.Value.filled)
            {
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().setOpponentDeck(getDeck(clientDeck.Value));
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().decideFirstPlayer(IsServer);
                Debug.Log("Started the first turn");
                done = true;
            }
            else if (!IsServer && serverDeck.Value.filled)
            {
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().setOpponentDeck(getDeck(serverDeck.Value));
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().decideFirstPlayer(IsServer);
                Debug.Log("Started the first turn");
                done = true;
            }
        }
        else
        {
            if (IsServer && clientPlayerAction.Value.actionNum > actionsReceivedFromClient)
            {
                PlayerAction data = clientPlayerAction.Value;
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().acceptOpponentAction(data.actionType, data.position, data.attackerIdx, data.defenderIdx);
                actionsReceivedFromClient++;
            }
            else if (!IsServer && serverPlayerAction.Value.actionNum > actionsReceivedFromServer)
            {
                PlayerAction data = serverPlayerAction.Value;
                GameObject.Find(GAMEBOARD_NAME)
                    .GetComponent<Gameboard>().acceptOpponentAction(data.actionType, data.position, data.attackerIdx, data.defenderIdx);
                actionsReceivedFromServer++;
            }
        }
    }
    private string[] getDeck(DeckSendData deckData)
    {
        return new string[]
        {
            deckData.card0.ToString(),
            deckData.card1.ToString(),
            deckData.card2.ToString(),
            deckData.card3.ToString(),
            deckData.card4.ToString(),
            deckData.card5.ToString(),
            deckData.card6.ToString(),
            deckData.card7.ToString(),
            deckData.card8.ToString(),
            deckData.card9.ToString(),
            deckData.card0.ToString(),
            deckData.card11.ToString(),
            deckData.card12.ToString(),
            deckData.card13.ToString(),
            deckData.card14.ToString(),
            deckData.card15.ToString(),
            deckData.card16.ToString(),
            deckData.card17.ToString(),
            deckData.card18.ToString(),
            deckData.card19.ToString(),
            deckData.card20.ToString(),
            deckData.card21.ToString(),
            deckData.card22.ToString(),
            deckData.card23.ToString(),
            deckData.card24.ToString(),
            deckData.card25.ToString(),
            deckData.card26.ToString(),
            deckData.card27.ToString(),
            deckData.card28.ToString(),
            deckData.card29.ToString(),
            deckData.card30.ToString(),
            deckData.card31.ToString(),
            deckData.card32.ToString(),
            deckData.card33.ToString(),
            deckData.card34.ToString(),
            deckData.card35.ToString(),
            deckData.card36.ToString(),
            deckData.card37.ToString(),
            deckData.card38.ToString(),
            deckData.card39.ToString(),
        };
    }

    public struct DeckSendData : INetworkSerializable
    {
        public bool filled;
        public FixedString32Bytes card0;
        public FixedString32Bytes card1;
        public FixedString32Bytes card2;
        public FixedString32Bytes card3;
        public FixedString32Bytes card4;
        public FixedString32Bytes card5;
        public FixedString32Bytes card6;
        public FixedString32Bytes card7;
        public FixedString32Bytes card8;
        public FixedString32Bytes card9;
        public FixedString32Bytes card10;
        public FixedString32Bytes card11;
        public FixedString32Bytes card12;
        public FixedString32Bytes card13;
        public FixedString32Bytes card14;
        public FixedString32Bytes card15;
        public FixedString32Bytes card16;
        public FixedString32Bytes card17;
        public FixedString32Bytes card18;
        public FixedString32Bytes card19;
        public FixedString32Bytes card20;
        public FixedString32Bytes card21;
        public FixedString32Bytes card22;
        public FixedString32Bytes card23;
        public FixedString32Bytes card24;
        public FixedString32Bytes card25;
        public FixedString32Bytes card26;
        public FixedString32Bytes card27;
        public FixedString32Bytes card28;
        public FixedString32Bytes card29;
        public FixedString32Bytes card30;
        public FixedString32Bytes card31;
        public FixedString32Bytes card32;
        public FixedString32Bytes card33;
        public FixedString32Bytes card34;
        public FixedString32Bytes card35;
        public FixedString32Bytes card36;
        public FixedString32Bytes card37;
        public FixedString32Bytes card38;
        public FixedString32Bytes card39;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref filled);
            serializer.SerializeValue(ref card0);
            serializer.SerializeValue(ref card1);
            serializer.SerializeValue(ref card2);
            serializer.SerializeValue(ref card3);
            serializer.SerializeValue(ref card4);
            serializer.SerializeValue(ref card5);
            serializer.SerializeValue(ref card6);
            serializer.SerializeValue(ref card7);
            serializer.SerializeValue(ref card8);
            serializer.SerializeValue(ref card9);
            serializer.SerializeValue(ref card10);
            serializer.SerializeValue(ref card11);
            serializer.SerializeValue(ref card12);
            serializer.SerializeValue(ref card13);
            serializer.SerializeValue(ref card14);
            serializer.SerializeValue(ref card15);
            serializer.SerializeValue(ref card16);
            serializer.SerializeValue(ref card17);
            serializer.SerializeValue(ref card18);
            serializer.SerializeValue(ref card19);
            serializer.SerializeValue(ref card20);
            serializer.SerializeValue(ref card21);
            serializer.SerializeValue(ref card22);
            serializer.SerializeValue(ref card23);
            serializer.SerializeValue(ref card24);
            serializer.SerializeValue(ref card25);
            serializer.SerializeValue(ref card26);
            serializer.SerializeValue(ref card27);
            serializer.SerializeValue(ref card28);
            serializer.SerializeValue(ref card29);
            serializer.SerializeValue(ref card30);
            serializer.SerializeValue(ref card31);
            serializer.SerializeValue(ref card32);
            serializer.SerializeValue(ref card33);
            serializer.SerializeValue(ref card34);
            serializer.SerializeValue(ref card35);
            serializer.SerializeValue(ref card36);
            serializer.SerializeValue(ref card37);
            serializer.SerializeValue(ref card38);
            serializer.SerializeValue(ref card39);
        }

    }

    public struct PlayerAction : INetworkSerializable
    {
        public int actionNum;
        public int actionType;
        public int position;
        public int attackerIdx;
        public int defenderIdx;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref actionNum);
            serializer.SerializeValue(ref actionType);
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref attackerIdx);
            serializer.SerializeValue(ref defenderIdx);
        }
    }

    public void setAction(int actionType, int position, int attackerIdx, int defenderIdx)
    {
        if (IsServer)
        {
            actionsSentFromServer++;

            PlayerAction data = new PlayerAction();
            data.actionNum = actionsSentFromServer;
            data.actionType = actionType;
            data.position = position;
            data.attackerIdx = attackerIdx;
            data.defenderIdx = defenderIdx;

            serverPlayerAction.Value = data;
        }
        else
        {
            /*
            actionsSentFromClient++;

            PlayerAction data = new PlayerAction();
            data.actionNum = actionsSentFromClient;
            data.actionType = actionType;
            data.position = position;
            data.attackerIdx = attackerIdx;
            data.defenderIdx = defenderIdx;

            clientPlayerAction.Value = data;
            */
            PlayerActionServerRpc(actionType, position, attackerIdx, defenderIdx);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeckServerRpc(DeckSendData data)
    {
        Debug.Log("server received rpc");
        clientDeck.Value = data;
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayerActionServerRpc(int actionType, int position, int attackerIdx, int defenderIdx)
    {
        actionsSentFromClient++;

        PlayerAction data = new PlayerAction();
        data.actionNum = actionsSentFromClient;
        data.actionType = actionType;
        data.position = position;
        data.attackerIdx = attackerIdx;
        data.defenderIdx = defenderIdx;

        clientPlayerAction.Value = data;
    }

}
