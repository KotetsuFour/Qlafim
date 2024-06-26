using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveMechanism
{
    public static string saveData = "Assets/PlayerData/playerfile.save";
    public static string dlcCardFolder = "Assets/DLCCards/";
    public static string dlcMakerFolder = "Assets/DLC/";
    public static void savePlayerData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveData, FileMode.Create);

        formatter.Serialize(stream, StaticData.playerData);

        stream.Close();
    }

    public static void loadGame()
    {
        if (File.Exists(saveData))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveData, FileMode.Open);

            StaticData.playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
        }
        else
        {
            StaticData.playerData = new PlayerData();
        }
    }

    public static bool hasFile(string fileName)
    {
        return File.Exists(fileName);
    }

    public static void importDLCCards(Human h, NonHuman n)
    {
        string[] files = Directory.GetFiles(dlcCardFolder);
        foreach (string file in files)
        {
            if (file.EndsWith(".qlaf"))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(file, FileMode.Open);

                    CardsData data = formatter.Deserialize(stream) as CardsData;
                    stream.Close();

                    List<TradingCard> cards = data.getAllCards(h, n);
                    StaticData.playerData.addCards(cards);
                    foreach (TradingCard card in cards)
                    {
                        Object.Destroy(card);
                    }

                    deleteFile(file);

                    savePlayerData();
                }
                catch (IOException ex)
                {
                    Debug.Log($"IOException when reading {file}: {ex.Message}");
                }
            }
        }
    }

    public static void makeCardPack(string packName, List<TradingCard> cards)
    {
        CardsData pack = new CardsData();
        pack.addCards(cards);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = $"{dlcMakerFolder}{packName}.qlaf";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, pack);

        stream.Close();
    }

    public static void deleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
