using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject menuDisplay;
    [SerializeField] private bool testing;

    [SerializeField] private Button cardButton;
    [SerializeField] private Button deckButton;

    private string screen;

    // Start is called before the first frame update
    void Start()
    {
        if (!StaticData.loaded)
        {
            StaticNetworking.initialize();

            StaticData.myCards = new Dictionary<string, int>();
            StaticData.myDecks = new List<string[]>();
            StaticData.myDeckNames = new List<string>();

            SaveMechanism.loadGame();
            StaticData.loaded = true;
        }
        StaticData.findDeepChild(menuDisplay.transform, "ProfileImage").GetComponent<Image>().sprite
            = TradingCard.createImageFromByteArray(StaticData.playerData.profileImage);
        StaticData.findDeepChild(menuDisplay.transform, "PlayerName").GetComponent<TextMeshProUGUI>().text
            = StaticData.playerData.playerName;
        if (!StaticData.playerData.initialized)
        {
            switchToScreen("PickFirstDeck");
        }
        else
        {
            switchToScreen("MainMenu");
        }
    }

    public void switchToScreen(string objectName)
    {
        for (int q = 0; q < menuDisplay.transform.childCount; q++)
        {
            menuDisplay.transform.GetChild(q).gameObject.SetActive(false);
        }
        screen = objectName;
        if (screen != null)
        {
            StaticData.findDeepChild(menuDisplay.transform, screen).gameObject.SetActive(true);
        }
    }

    public void pickGospels()
    {
        if (testing)
        {
            StaticData.myCards.Add("TEST-HUMAN", 40);
            StaticData.myCards.Add("TEST-NONHUMAN", 40);
            string[] testDeck = new string[StaticData.NUM_CARDS_IN_DECK];
            for (int q = 0; q < StaticData.NUM_CARDS_IN_DECK / 2; q++)
            {
                testDeck[q] = "TEST-HUMAN";
            }
            for (int q = StaticData.NUM_CARDS_IN_DECK / 2; q < StaticData.NUM_CARDS_IN_DECK; q++)
            {
                testDeck[q] = "TEST-NONHUMAN";
            }
            StaticData.myDecks.Add(testDeck);
        }
        else
        {
            //TODO unlock the gospels deck
        }
        StaticData.playerData.initialized = true;
        switchToScreen("PickName");
    }
    public void pickGenesis()
    {
        if (testing)
        {
            StaticData.myCards.Add("TEST-HUMAN", 40);
            StaticData.myCards.Add("TEST-NONHUMAN", 40);
            string[] testDeck = new string[StaticData.NUM_CARDS_IN_DECK];
            for (int q = 0; q < StaticData.NUM_CARDS_IN_DECK / 2; q++)
            {
                testDeck[q] = "TEST-HUMAN";
            }
            for (int q = StaticData.NUM_CARDS_IN_DECK / 2; q < StaticData.NUM_CARDS_IN_DECK; q++)
            {
                testDeck[q] = "TEST-NONHUMAN";
            }
            StaticData.myDecks.Add(testDeck);
        }
        else
        {
            //TODO unlock the Genesis deck
        }
        StaticData.playerData.initialized = true;
        switchToScreen("PickName");
    }

    public void setName()
    {
        Transform field = StaticData.findDeepChild(menuDisplay.transform, "NameField");
        if (field.GetComponent<InputRestricter>().isValid())
        {
            StaticData.playerData.playerName = field.GetComponent<TMP_InputField>().text.Trim();
            StaticData.findDeepChild(menuDisplay.transform, "PlayerName").GetComponent<TextMeshProUGUI>().text
                = StaticData.playerData.playerName;
            switchToScreen("MainMenu");
        } else
        {
            StaticData.findDeepChild(menuDisplay.transform, "NameError").gameObject.SetActive(true);
        }
    }

    public void myCollection()
    {
        switchToScreen("CollectionPage");
        Transform cardsTransform = StaticData.findDeepChild(menuDisplay.transform, "CardCollectionContent");
        foreach (string id in StaticData.myCards.Keys)
        {
            Button card = Instantiate(cardButton, cardsTransform);
            Button.ButtonClickedEvent examine = new Button.ButtonClickedEvent();
            examine.AddListener(delegate { examineCard(id); });
            card.GetComponent<Image>().sprite = CardDictionary.getCard(id).getFaceImage();
            StaticData.findDeepChild(card.transform, "Amount").GetComponent<TextMeshProUGUI>()
                .text = "" + StaticData.myCards[id];
        }
    }

    public void examineCard(string id)
    {

    }

    public void startNewGame()
    {
        StaticNetworking.createRelay(StaticData.findDeepChild(menuDisplay.transform, "JoinCodeDisplay")
            .GetComponent<TextMeshProUGUI>());
        switchToScreen("Lobby");
    }
    public void joinGame()
    {
        string code = StaticData.findDeepChild(menuDisplay.transform, "JoinCode")
            .GetComponent<TMP_InputField>().text;
        StaticNetworking.joinRelay(code);
    }
    public void cancelGame()
    {
        StaticNetworking.cancelConnection();
        switchToScreen("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        StaticNetworking.switchScenesWhenReady();
    }
}
