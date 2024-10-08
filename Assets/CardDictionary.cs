using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDictionary : MonoBehaviour
{
    [SerializeField] private TradingCard adam;
    [SerializeField] private TradingCard eve;
    [SerializeField] private TradingCard cain;
    [SerializeField] private TradingCard abel;
    [SerializeField] private TradingCard lamech;

    [SerializeField] private TradingCard jesus;

    [SerializeField] private TradingCard testHuman;
    [SerializeField] private TradingCard testNonHuman;

    public static Dictionary<string, TradingCard> cardDictionary;

    // Start is called before the first frame update
    void Start()
    {
        cardDictionary = new Dictionary<string, TradingCard>();
        setupTesting();
        setupGenesis();
        setupGospels();
    }

    public void setupGenesis()
    {
        //Adam
        adam.allowTriggers = noTriggers();
        adam.allowCode = new string[] { };
        adam.reactTriggers = noTriggers();
        adam.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        adam.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        adam.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"STORELENGTH 0 0.0",
            /*7*/"SUB -1 1",
            /*8*/"LES -1 0 11 <JUMPTOEND>",
            /*9*/"ISHUMAN 0.0.-1 12 <SKIPRETURN>",
            /*10*/"JUMP 7",
            /*11*/"RETURN",
            /*12*/"NOTE 9 | 0.0.-1 | 0.0 0.1",
            /*13*/"RETURN"
        };
        addCardToDictionary(adam);

        //Eve
        eve.allowTriggers = noTriggers();
        eve.allowCode = new string[] { };
        eve.reactTriggers = noTriggers();
        eve.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        eve.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        eve.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"STORELENGTH 0 0.1",
            /*7*/"SUB -1 1",
            /*8*/"LES -1 0 11 <JUMPTOEND>",
            /*9*/"ISHUMAN 0.1.-1 12 <SKIPRETURN>",
            /*10*/"JUMP 7",
            /*11*/"RETURN",
            /*12*/"STORECARD 0 0.1.-1",
            /*13*/"NOTE 9 | STORE.0 | 0.1 0.2",
            /*14*/"NOTE 1 1 2 | STORE.0 |",
            /*15*/"RETURN"
        };
        addCardToDictionary(eve);

        //Cain
        cain.allowTriggers = noTriggers();
        cain.allowCode = new string[] { };
        cain.reactTriggers = noTriggers();
        cain.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        cain.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        cain.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"STORELENGTH 0 1.2",
            /*7*/"SUB -1 1",
            /*8*/"LES -1 0 11 <JUMPTOENDOFLOOP>",
            /*9*/"GRT ATK 1.2.-1.ATK 12 <JUMPTOEFFECT>",
            /*10*/"JUMP 7",
            /*11*/"RETURN",
            /*12*/"NOTE 9 | 1.2.-1 | 1.2 1.4",
            /*13*/"RETURN"
        };
        addCardToDictionary(cain);

        //Abel
        abel.allowTriggers = noTriggers();
        abel.allowCode = new string[] { };
        abel.reactTriggers = noTriggers();
        abel.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        abel.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        abel.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"STORELENGTH 0 1.1",
            /*7*/"SUB -1 1",
            /*8*/"LES -1 0 11 <JUMPTOENDOFLOOP>",
            /*9*/"ISHUMAN 1.1.-1 12 <JUMPTOEFFECT>",
            /*10*/"JUMP 7",
            /*11*/"RETURN",
            /*12*/"NOTE 2 100 | THIS |",
            /*13*/"NOTE 2 1.1.-1.ATK | THIS |",
            /*14*/"RETURN"
        };
        addCardToDictionary(abel);

        //Lamech
        lamech.allowTriggers = noTriggers();
        lamech.allowCode = new string[] { };
        lamech.reactTriggers = noTriggers();
        lamech.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        lamech.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        lamech.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"STORELENGTH 0 1.1",
            /*7*/"SUB -1 1",
            /*8*/"LES -1 0 13 <JUMPTOENDOFLOOP>",
            /*9*/"GRT ATK 1.2.-1.ATK 11 <JUMPTOEFFECT>",
            /*10*/"JUMP 7",
            /*11*/"NOTE 9 | 1.2.-1 | 1.2 1.4",
            /*12*/"JUMP 7",
            /*13*/"RETURN"
        };
        addCardToDictionary(lamech);
    }

    public void setupGospels()
    {
        //Jesus
        jesus.allowTriggers = noTriggers();
        jesus.allowCode = new string[] { };
        jesus.reactTriggers = noTriggers();
        jesus.reactTriggers[(int)GameNotification.Nature.PLAYER_ACTION] = 0;
        jesus.reactTriggers[(int)GameNotification.Nature.ACTIVATE_ABILITY] = 5;
        jesus.reactTriggers[(int)GameNotification.Nature.MOVE_CARD] = 8;
        jesus.reactCode = new string[]
        {
            /*0*/"// Check to see if the card is this REACT TO REVEAL STARTS HERE",
            /*1*/"ISREVEAL 3 // Fourth line, skipping the RETURN statement",
            /*2*/"RETURN",
            /*3*/"NOTE 10 | THIS |",
            /*4*/"RETURN",
            /*5*/"// Move Jesus to graveyard REACT TO ACTIVATE ABILITY STARTS HERE",
            /*6*/"NOTE 9 | THIS | 0.2 0.4",
            /*7*/"RETURN",
            /*8*/"// If successfully moved Jesus to graveyard, change all other cards to 1 Cost. REACT TO MOVE STARTS HERE",
            /*9*/"ISCAUSE 11",
            /*10*/"RETURN",
            /*11*/"CARDEQUALS NOTE.0 THIS 13",
            /*12*/"RETURN",
            /*13*/"POSITIONEQUALS 0.4 HERE 15",
            /*14*/"POSITIONEQUALS 0.1 HERE 33 <MOVEOTHERCARDS>",
            /*15*/"RETURN",
            /*16*/"STOREINT 0 0",
            /*17*/"EQL -1 6 30 <JUMPTOENDOFLOOP>",
            /*18*/"STORELENGTH 1 0.-1",
            /*19*/"STOREINT 2 0",
            /*20*/"EQL -3 -2 28 <JUMPENDOFINNERLOOP>",
            /*21*/"STORECARD 0 0.-1.-3",
            /*22*/"CARDVALUEEQUALS STORE.0 THIS 25 <JUMPTO8MAKER>",
            /*23*/"NOTE 11 1 | STORE.0 |",
            /*24*/"JUMP 26 <TOLOOPEND>",
            /*25*/"NOTE 11 8 | STORE.0 |",
            /*26*/"ADD -3 1",
            /*27*/"JUMP 20",
            /*28*/"ADD -1 1",
            /*29*/"JUMP 17",
            /*30*/"// Move this from the graveyard to your hand",
            /*31*/"NOTE 9 | THIS | 0.4 0.1",
            /*32*/"RETURN",
            /*33*/"// Move graveyard to the hand",
            /*34*/"STORELENGTH 0 0.4",
            /*35*/"SUB -1 1",
            /*36*/"LES -1 0 39 <JUMPTOLOOPEND>",
            /*37*/"NOTE 9 | 0.4.-1 | 0.4 0.1",
            /*38*/"JUMP 35",
            /*39*/"RETURN",
        };
        addCardToDictionary(jesus);
    }
    public void setupTesting()
    {
        testHuman.allowTriggers = noTriggers();
        testHuman.allowCode = new string[] { };
        testHuman.reactTriggers = noTriggers();
        testHuman.reactCode = new string[] { };

        addCardToDictionary(testHuman);

        testNonHuman.allowTriggers = noTriggers();
        testNonHuman.allowCode = new string[] { };
        testNonHuman.reactTriggers = noTriggers();
        testNonHuman.reactCode = new string[] { };

        addCardToDictionary(testNonHuman);
    }

    private void addCardToDictionary(TradingCard card)
    {
        cardDictionary.Add(card.cardID, card);
    }

    public static TradingCard getCard(string key)
    {
        return cardDictionary[key];
    }
    private int[] noTriggers()
    {
        int[] ret = new int[GameNotification.NOTIFICATION_TYPES];
        for (int q = 0; q < ret.Length; q++)
        {
            ret[q] = int.MaxValue;
        }
        return ret;
    }
}
