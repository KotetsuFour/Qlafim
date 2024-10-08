using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveMechanism
{
    public static string saveData = "Assets/PlayerData/playerfile.qlaf";
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

}
