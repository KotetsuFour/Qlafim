using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gameboard : NotificationHandler
{
    [SerializeField] private Deck myDeck;
    [SerializeField] private PositionState myHand;
    [SerializeField] private PositionState myHumans;
    [SerializeField] private PositionState myNonHumans;
    [SerializeField] private PositionState myGraveyard;
    [SerializeField] private PositionState myWilderness;
    public PositionState[] myPositionStates;

    [SerializeField] private Deck yourDeck;
    [SerializeField] private PositionState yourHand;
    [SerializeField] private PositionState yourHumans;
    [SerializeField] private PositionState yourNonHumans;
    [SerializeField] private PositionState yourGraveyard;
    [SerializeField] private PositionState yourWilderness;
    public PositionState[] yourPositionStates;

    private int myEnergy;
    private int yourEnergy;

    private int myLifePoints;
    private int yourLifePoints;

    private LinkedList<GameNotification> notificationQueue;
    private LinkedListNode<GameNotification> addAfter;

    [SerializeField] private string boardName;
    [SerializeField] private string initiative;

    private Phase currentPhase;
    private bool myTurn;

    [SerializeField] private Button next;
    [SerializeField] private Button action1;
    [SerializeField] private Button action2;

    [SerializeField] private LayerMask selectableLayer;

    private GameObject selectedObject;

    [SerializeField] private Camera cam;

    [SerializeField] private Human human;
    [SerializeField] private NonHuman nonHuman;

    private int[] opponentShuffleOrder;

    // Start is called before the first frame update
    void Start()
    {
        StaticData.board = this;
        notificationQueue = new LinkedList<GameNotification>();

        myPositionStates = new PositionState[] {
            /*[0]*/myDeck,
            /*[1]*/myHand,
            /*[2]*/myHumans,
            /*[3]*/myNonHumans,
            /*[4]*/myGraveyard,
            /*[5]*/myWilderness
        };
        yourPositionStates = new PositionState[] {
            /*[0]*/yourDeck,
            /*[1]*/yourHand,
            /*[2]*/yourHumans,
            /*[3]*/yourNonHumans,
            /*[4]*/yourGraveyard,
            /*[5]*/yourWilderness
        };
    }
    public CardsData getMyDeckData()
    {
        return new CardsData(myDeck.cardsHere);
    }

    public void setOpponentShuffleOrder(int[] order)
    {
        opponentShuffleOrder = order;
    }
    public void setDecks(CardsData opponentData, int[] myShuffleOrder)
    {
        List<TradingCard> myShuffledDeck = new List<TradingCard>();
        for (int q = 0; q < myShuffleOrder.Length; q++)
        {
            myShuffledDeck.Add(myDeck.cardsHere[myShuffleOrder[q]]);
        }
        myDeck.cardsHere.Clear();
        foreach (TradingCard card in myShuffledDeck)
        {
            myDeck.addCard(card);
        }

        List<TradingCard> deck = opponentData.getAllCards(human, nonHuman);
        List<TradingCard> yourShuffledDeck = new List<TradingCard>();
        for (int q = 0; q < opponentShuffleOrder.Length; q++)
        {
            yourShuffledDeck.Add(deck[opponentShuffleOrder[q]]);
        }
        foreach (TradingCard card in yourShuffledDeck)
        {
            yourDeck.addCard(card);
        }
    }

    public void decideFirstPlayer(bool isHost)
    {
        int[] myAttributesCount = new int[7];
        int[] myCostCounts = new int[9];
        int myTotalCost = 0;

        int[] yourAttributesCount = new int[7];
        int[] yourCostCounts = new int[9];
        int yourTotalCost = 0;

        foreach (TradingCard card in myDeck.cardsHere)
        {
            myAttributesCount[(int)card.attribute]++;
            if (card is Human)
            {
                Human hum = (Human)card;
                myCostCounts[hum.energyCost.value]++;
                myTotalCost += hum.energyCost.value;
            }
        }
        foreach (TradingCard card in yourDeck.cardsHere)
        {
            yourAttributesCount[(int)card.attribute]++;
            if (card is Human)
            {
                Human hum = (Human)card;
                yourCostCounts[hum.energyCost.value]++;
                yourTotalCost += hum.energyCost.value;
            }
        }

        string[] parts = initiative.Split(" ");
        if (parts[0] == ">")
        {
            if (parts[1] == "ATTRIBUTE")
            {
                int attr = int.Parse(parts[2]);
                if (myAttributesCount[attr] == yourAttributesCount[attr])
                {
                    myTurn = isHost;
                }
                else if (myAttributesCount[attr] > yourAttributesCount[attr])
                {
                    myTurn = true;
                }
            }
            else if (parts[1] == "COST")
            {
                int cost = int.Parse(parts[2]);
                if (myCostCounts[cost] == yourCostCounts[cost])
                {
                    myTurn = isHost;
                }
                else if (myCostCounts[cost] > yourCostCounts[cost])
                {
                    myTurn = true;
                }
            }
            else if (parts[1] == "TOTALCOST")
            {
                if (myTotalCost == yourTotalCost)
                {
                    myTurn = isHost;
                }
                else if (myTotalCost > yourTotalCost)
                {
                    myTurn = true;
                }
            }
        }
        else if (parts[0] == "<")
        {
            if (parts[1] == "ATTRIBUTE")
            {
                int attr = int.Parse(parts[2]);
                if (myAttributesCount[attr] == yourAttributesCount[attr])
                {
                    myTurn = isHost;
                }
                else if (myAttributesCount[attr] < yourAttributesCount[attr])
                {
                    myTurn = true;
                }
            }
            else if (parts[1] == "COST")
            {
                int cost = int.Parse(parts[2]);
                if (myCostCounts[cost] == yourCostCounts[cost])
                {
                    myTurn = isHost;
                }
                else if (myCostCounts[cost] < yourCostCounts[cost])
                {
                    myTurn = true;
                }
            }
            else if (parts[1] == "TOTALCOST")
            {
                if (myTotalCost == yourTotalCost)
                {
                    myTurn = isHost;
                }
                else if (myTotalCost < yourTotalCost)
                {
                    myTurn = true;
                }
            }
        }
    }

    public PositionState[] getPositionStatesByCard(TradingCard card, bool mine)
    {
        foreach (PositionState state in myPositionStates)
        {
            if (card.positionState == state)
            {
                if (mine)
                {
                    return myPositionStates;
                }
                else
                {
                    break;
                }
            }
        }
        return yourPositionStates;
    }
    public Phase getCurrentPhase()
    {
        return currentPhase;
    }
    public bool isMyTurn()
    {
        return myTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if (notificationQueue.Count > 0)
        {
            if (notificationQueue.First.Value.isDone())
            {
                if (!notificationQueue.First.Value.wasDenied())
                {
                    addAfter = notificationQueue.First;
                    if (notificationQueue.First.Value.nature == GameNotification.Nature.ACTIVATE_ABILITY)
                    {
                        notificationQueue.First.Value.cardRegistry[0].react(notificationQueue.First.Value);
                    }
                    else
                    {
                        List<TradingCard> allCards = getAllCardsInGame();
                        foreach (TradingCard card in allCards)
                        {
                            card.react(notificationQueue.First.Value);
                        }
                        react(notificationQueue.First.Value);
                    }
                }
                notificationQueue.RemoveFirst();
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, selectableLayer))
                {
                    selectedObject = hit.collider.gameObject;
                    
                    if (selectedObject.GetComponent<TradingCard>() != null)
                    {
                        TradingCard selectedCard = selectedObject.GetComponent<TradingCard>();
                        if (myHand.cardsHere.Contains(selectedCard))
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = selectedCard.transform.position + new Vector3(0, 1 * selectedCard.transform.localScale.y, 0);
                            cardInHandOptions(selectedCard);
                        }
                        else if (myHumans.cardsHere.Contains(selectedCard))
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = selectedCard.transform.position + new Vector3(0, 1 * selectedCard.transform.localScale.y, 0);
                            humanOnFieldOptions((Human)selectedCard);
                        }
                        else if (myNonHumans.cardsHere.Contains(selectedCard))
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = selectedCard.transform.position + new Vector3(0, 1 * selectedCard.transform.localScale.y, 0);
                            nonHumanOnFieldOptions((NonHuman)selectedCard);
                        }
                        else if ((yourHumans.cardsHere.Contains(selectedCard)
                            || yourNonHumans.cardsHere.Contains(selectedCard))
                            && selectedCard.position != 0)
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = selectedCard.transform.position + new Vector3(0, 1 * selectedCard.transform.localScale.y, 0);
                            examineOnlyOptions();
                        }
                        else //No selection allowed
                        {
                            hideSelectionOptions();
                        }
                    }
                    else if (selectedObject.GetComponent<PositionState>() != null)
                    {
                        PositionState positionState = selectedObject.GetComponent<PositionState>();
                        if (positionState == myGraveyard)
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = positionState.transform.position + new Vector3(0, 1 * positionState.transform.localScale.y, 0);
                            //TODO graveyard options
                        }
                        else if (positionState == myWilderness)
                        {
                            StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(true);
                            StaticData.findDeepChild(transform, "Indicator").position = positionState.transform.position + new Vector3(0, 1 * positionState.transform.localScale.y, 0);
                            //TODO wilderness options
                        }
                    }
                }
            }
        }
    }
    private void hideSelectionOptions()
    {
        StaticData.findDeepChild(transform, "Indicator").gameObject.SetActive(false);
        selectedObject = null;
        if (myTurn)
        {
            next.gameObject.SetActive(true);
            next.interactable = true;

            if (currentPhase == Phase.MAIN2)
            {
                StaticData.findDeepChild(next.transform, "ButtonName")
                    .GetComponent<TextMeshProUGUI>().text = "END";
            }
            else
            {
                StaticData.findDeepChild(next.transform, "ButtonName")
                    .GetComponent<TextMeshProUGUI>().text = $"{currentPhase + 1}";
            }

            Button.ButtonClickedEvent phase = new Button.ButtonClickedEvent();
            phase.AddListener(changePhase);
            next.onClick = phase;
        }
        else
        {
            next.gameObject.SetActive(false);
        }
        action1.gameObject.SetActive(false);
        action2.gameObject.SetActive(false);
    }

    private void changePhase()
    {
        GameNotification note = new GameNotification(GameNotification.Nature.GAME_STATE, false, this);
        notificationQueue.AddLast(note);
    }
    private void cardInHandOptions(TradingCard selectedCard)
    {
        action1.gameObject.SetActive(true);
        action2.gameObject.SetActive(true);
        next.gameObject.SetActive(true);
        action1.interactable = true;
        action2.interactable = true;
        next.interactable = true;

        StaticData.findDeepChild(action1.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "PLAY";
        StaticData.findDeepChild(action2.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "EXAMINE";
        StaticData.findDeepChild(next.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "CANCEL";


        Button.ButtonClickedEvent play = new Button.ButtonClickedEvent();
        if (!myTurn || (currentPhase != Phase.MAIN1 && currentPhase != Phase.MAIN2)
            || (selectedCard is Human && ((Human)selectedCard).energyCost.effectiveValue() > myEnergy))
        {
            action1.interactable = false;
        }
        else if (selectedCard is NonHuman)
        {
            play.AddListener(playNonHumanCard);
        }
        else if (selectedCard is Human)
        {
            play.AddListener(playHumanCardOptions);
        }
        action1.onClick = play;

        Button.ButtonClickedEvent examine = new Button.ButtonClickedEvent();
        examine.AddListener(examineCard);
        action2.onClick = examine;

        Button.ButtonClickedEvent cancel = new Button.ButtonClickedEvent();
        cancel.AddListener(hideSelectionOptions);
        next.onClick = cancel;
    }
    private void playHumanCardOptions()
    {
        action1.gameObject.SetActive(true);
        action2.gameObject.SetActive(true);
        next.gameObject.SetActive(true);
        action1.interactable = true;
        action2.interactable = true;
        next.interactable = true;

        Human selectedCard = selectedObject.GetComponent<Human>();

        StaticData.findDeepChild(action1.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "ATK";
        StaticData.findDeepChild(action2.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "DEF";
        StaticData.findDeepChild(next.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "CANCEL";

        Button.ButtonClickedEvent playATK = new Button.ButtonClickedEvent();
        playATK.AddListener(playHumanATK);
        action1.onClick = playATK;

        Button.ButtonClickedEvent playDEF = new Button.ButtonClickedEvent();
        playDEF.AddListener(playHumanDEF);
        action2.onClick = playDEF;

        Button.ButtonClickedEvent cancel = new Button.ButtonClickedEvent();
        cancel.AddListener(hideSelectionOptions);
        next.onClick = cancel;
    }
    private void playHumanATK()
    {
        GameNotification note = new GameNotification(GameNotification.Nature.PLAYER_ACTION, true, this);
        note.cardRegistry.Add(selectedObject.GetComponent<TradingCard>());
        note.integerRegistry.Add(new IntegerRegister(0));
        note.integerRegistry.Add(new IntegerRegister(2));
        addNotification(note);
        hideSelectionOptions();
    }
    private void playHumanDEF()
    {
        GameNotification note = new GameNotification(GameNotification.Nature.PLAYER_ACTION, true, this);
        note.cardRegistry.Add(selectedObject.GetComponent<TradingCard>());
        note.integerRegistry.Add(new IntegerRegister(0));
        note.integerRegistry.Add(new IntegerRegister(1));
        addNotification(note);
        hideSelectionOptions();
    }
    private void playNonHumanCard()
    {
        GameNotification note = new GameNotification(GameNotification.Nature.PLAYER_ACTION, true, this);
        note.cardRegistry.Add(selectedObject.GetComponent<TradingCard>());
        note.integerRegistry.Add(new IntegerRegister(0));
        addNotification(note);
        hideSelectionOptions();
    }
    private void examineCard()
    {
        action1.gameObject.SetActive(false);
        action2.gameObject.SetActive(false);
        next.gameObject.SetActive(false);

        TradingCard card = selectedObject.GetComponent<TradingCard>();
        StaticData.findDeepChild(transform, "ViewCard").gameObject.SetActive(true);
        StaticData.findDeepChild(transform, "CardView").GetComponent<Image>().sprite
            = card.getFaceImage();
        StaticData.findDeepChild(transform, "AbilityDesc").GetComponent<TextMeshProUGUI>().text
            = card.abilityDescription;
        Button done = StaticData.findDeepChild(transform, "FinishedViewingCard").GetComponent<Button>();
        Button.ButtonClickedEvent doneEvent = new Button.ButtonClickedEvent();
        doneEvent.AddListener(finishedExamining);
        done.onClick = doneEvent;
    }
    private void finishedExamining()
    {
        StaticData.findDeepChild(transform, "ViewCard").gameObject.SetActive(false);
        hideSelectionOptions();
    }
    private void humanOnFieldOptions(Human selectedCard)
    {
        action1.gameObject.SetActive(true);
        action2.gameObject.SetActive(true);
        next.gameObject.SetActive(true);
        action1.interactable = true;
        action2.interactable = true;
        next.interactable = true;

        StaticData.findDeepChild(action1.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "ATK";
        StaticData.findDeepChild(action2.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "DEF";
        StaticData.findDeepChild(next.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "CANCEL";

        Button.ButtonClickedEvent setATK = new Button.ButtonClickedEvent();
        if (!myTurn || (currentPhase != Phase.MAIN1 && currentPhase != Phase.MAIN2)
            || selectedCard.position == 2)
        {
            action1.interactable = false;
        }
        else
        {
            setATK.AddListener(changeToATK);
        }
        action1.onClick = setATK;

        Button.ButtonClickedEvent setDEF = new Button.ButtonClickedEvent();
        if (!myTurn || (currentPhase != Phase.MAIN1 && currentPhase != Phase.MAIN2)
            || selectedCard.position == 1)
        {
            action2.interactable = false;
        }
        else
        {
            setDEF.AddListener(changeToDEF);
        }
        action2.onClick = setATK;

        Button.ButtonClickedEvent cancel = new Button.ButtonClickedEvent();
        cancel.AddListener(hideSelectionOptions);
        next.onClick = cancel;
    }
    private void nonHumanOnFieldOptions(NonHuman selectedCard)
    {
        action1.gameObject.SetActive(true);
        action2.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        action1.interactable = true;
        next.interactable = true;

        Button.ButtonClickedEvent examine = new Button.ButtonClickedEvent();
        examine.AddListener(examineCard);
        action1.onClick = examine;

        Button.ButtonClickedEvent cancel = new Button.ButtonClickedEvent();
        cancel.AddListener(hideSelectionOptions);
        next.onClick = cancel;
    }
    private void changeToATK()
    {
        Human selectedCard = selectedObject.GetComponent<Human>();
        GameNotification note = new GameNotification(GameNotification.Nature.PLAYER_ACTION, true, this);
        note.cardRegistry.Add(selectedCard);
        note.integerRegistry.Add(new IntegerRegister(1));
        note.integerRegistry.Add(new IntegerRegister(2));
        addNotification(note);
        hideSelectionOptions();
    }
    private void changeToDEF()
    {
        Human selectedCard = selectedObject.GetComponent<Human>();
        GameNotification note = new GameNotification(GameNotification.Nature.PLAYER_ACTION, true, this);
        note.cardRegistry.Add(selectedCard);
        note.integerRegistry.Add(new IntegerRegister(1));
        note.integerRegistry.Add(new IntegerRegister(1));
        addNotification(note);
        hideSelectionOptions();
    }
    private void examineOnlyOptions()
    {
        action1.gameObject.SetActive(true);
        action2.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        action1.interactable = true;
        next.interactable = true;

        StaticData.findDeepChild(action1.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "EXAMINE";
        StaticData.findDeepChild(next.transform, "ButtonName")
            .GetComponent<TextMeshProUGUI>().text = "CANCEL";

        Button.ButtonClickedEvent examine = new Button.ButtonClickedEvent();
        examine.AddListener(examineCard);
        action1.onClick = examine;

        Button.ButtonClickedEvent cancel = new Button.ButtonClickedEvent();
        cancel.AddListener(hideSelectionOptions);
        next.onClick = cancel;
    }
    public new void allowNotification(GameNotification note)
    {
        //nothing
    }
    public new void react(GameNotification note)
    {
        if (note.nature == GameNotification.Nature.GAME_STATE
            && currentPhase == Phase.DRAW)
        {
            if (myTurn)
            {
                GameNotification nrg = new GameNotification(GameNotification.Nature.CHANGE_ENERGY, true, this);
                nrg.integerRegistry.Add(new IntegerRegister(0));
                addNotification(nrg);

                GameNotification draw = new GameNotification(GameNotification.Nature.MOVE_CARD, true, this);
                draw.cardRegistry.Add(myDeck.get(myDeck.cardsHere.Count - 1));
                draw.positionStateRegistry.Add(myDeck);
                draw.positionStateRegistry.Add(myHand);
                addNotification(draw);
            }
            else
            {
                GameNotification nrg = new GameNotification(GameNotification.Nature.CHANGE_ENERGY, true, this);
                nrg.integerRegistry.Add(new IntegerRegister(1));
                addNotification(nrg);

                GameNotification draw = new GameNotification(GameNotification.Nature.MOVE_CARD, true, this);
                draw.cardRegistry.Add(yourDeck.get(yourDeck.cardsHere.Count - 1));
                draw.positionStateRegistry.Add(yourDeck);
                draw.positionStateRegistry.Add(yourHand);
                addNotification(draw);
            }
        }
        else if (note.nature == GameNotification.Nature.CHANGE_ENERGY)
        {
            StaticData.findDeepChild(transform, "Energy").GetComponent<TextMeshProUGUI>()
                .text = "" + myEnergy;
        }
        else if (note.nature == GameNotification.Nature.ALTER_LIFE_PTS)
        {
            StaticData.findDeepChild(transform, "MyLifePoints").GetComponent<TextMeshProUGUI>()
                .text = "" + myLifePoints;
            StaticData.findDeepChild(transform, "YourLifePoints").GetComponent<TextMeshProUGUI>()
                .text = "" + yourLifePoints;
            if (myLifePoints <= 0)
            {
                //TODO I lose
            }
            else if (yourLifePoints <= 0)
            {
                //TODO you lose
            }
        }
    }
    public void changePhaseNotificationCall(bool isMyTurn, Phase phase)
    {
        myTurn = isMyTurn;
        currentPhase = phase;
        //TODO if you're in the middle of examining a card, stay on that option panel
        hideSelectionOptions();
    }
    public override void processLine(string[] codeList)
    {
        //nothing
    }

    public List<TradingCard> getAllCardsInGame()
    {
        List<TradingCard> ret = new List<TradingCard>();
        if (myTurn)
        {
            foreach (PositionState pos in myPositionStates)
            {
                ret.AddRange(pos.cardsHere);
            }
        }
        else
        {
            foreach (PositionState pos in yourPositionStates)
            {
                ret.AddRange(pos.cardsHere);
            }
        }
        return ret;
    }

    public GameNotification getCurrentNotification()
    {
        return notificationQueue.First.Value;
    }
    public void addNotification(GameNotification note)
    {
        if (notificationQueue.Count <= 0)
        {
            notificationQueue.AddLast(note);
            addAfter = notificationQueue.First;
        }
        else
        {
            LinkedListNode<GameNotification> currentAddAfter = addAfter;
            notificationQueue.AddAfter(addAfter, note);
            addAfter = currentAddAfter.Next;
        }
    }
    public void denyCurrentNotification(NotificationHandler handler)
    {
        notificationQueue.First.Value.deny(handler);
    }

    public void alterEnergy(bool mine, int amount)
    {
        if (mine)
        {
            myEnergy += amount;
        }
        else
        {
            yourEnergy += amount;
        }
    }
    public void alterLifePoints(bool mine, int amount)
    {
        if (mine)
        {
            myLifePoints += amount;
        }
        else
        {
            yourLifePoints += amount;
        }
    }
    public enum Phase
    {
        DRAW, MAIN1, BATTLE, MAIN2
    }
}
