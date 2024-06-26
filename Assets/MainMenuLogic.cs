using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    [SerializeField] private Human humanPrefab;
    [SerializeField] private NonHuman nonHumanPrefab;
    [SerializeField] private GameObject menuDisplay;

    private string screen;

    // Start is called before the first frame update
    void Start()
    {
        if (!StaticData.loaded)
        {
            StaticNetworking.initialize();

            SaveMechanism.loadGame();
            SaveMechanism.importDLCCards(humanPrefab, nonHumanPrefab);
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
        //TODO unlock the gospels deck
        StaticData.playerData.initialized = true;
        switchToScreen("PickName");
    }
    public void pickGenesis()
    {
        //TODO unlock the Genesis deck
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

    public void startNewGame()
    {
        StaticNetworking.createRelay(StaticData.findDeepChild(menuDisplay.transform, "JoinCodeDisplay")
            .GetComponent<TextMeshProUGUI>());
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
