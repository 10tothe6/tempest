using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSession(SessionData sessionData)
    {
        string savePath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "sessionData", FileMode.Create);

        formatter.Serialize(stream, sessionData);
        stream.Close();

        Application.Quit();
    }

    public static SessionData LoadSession()
    {
        string loadPath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/";

        if (File.Exists(loadPath + "sessionData"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "sessionData", FileMode.Open);

            SessionData session = formatter.Deserialize(stream) as SessionData;
            stream.Close();
            return session;
        }
        else
        {
            return null;
        }
    }

    public static void SaveWorld(WorldData world)
    {
        string savePath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/" + GameObject.Find("UIManager").GetComponent<UIManager>().worldName + "/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "world", FileMode.Create);

        formatter.Serialize(stream, world);
        stream.Close();

        bool found = false;
        int set = -1;
        foreach (string currentName in GameObject.Find("UIManager").GetComponent<UIManager>().saveSlots)
        {
            if (currentName == world.worldName)
            {
                found = true;
            }
            else if (currentName == null && !found) 
            {
                set = System.Array.IndexOf(GameObject.Find("UIManager").GetComponent<UIManager>().saveSlots, currentName);
                found = true;
            }
        }
        if (set != -1)
        {
            GameObject.Find("UIManager").GetComponent<UIManager>().saveSlots[set] = world.worldName;
        }
    }

    public static WorldData LoadWorld(string worldName)
    {
        string loadPath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/" + worldName + "/";

        if (File.Exists(loadPath + "world"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "world", FileMode.Open);

            WorldData world = formatter.Deserialize(stream) as WorldData;
            stream.Close();
            return world;
        }
        else
        {
            return null;
        }
    }

    public static void SaveChunk(int posX, ChunkGeneration chunk, UIManager ui)
    {
        string savePath = ui.appPath + "/saves/" + "/Beta 1.0/" + ui.worldName + "/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "chunk" + posX, FileMode.Create);

        formatter.Serialize(stream, chunk.chunkData);
        stream.Close();
    }

    public static ChunkData LoadChunk(int posX)
    {
        string loadPath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/" + GameObject.Find("UIManager").GetComponent<UIManager>().worldName + "/";

        if (File.Exists(loadPath + "chunk" + posX))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "chunk" + posX, FileMode.Open);

            ChunkData chunkData = formatter.Deserialize(stream) as ChunkData;
            stream.Close();
            return chunkData;
        }
        else
        {
            return null;
        }
    }

    public static void SaveWorldSlots(string[] slotArray)
    {
        string savePath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/" + "/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath + "saveData", FileMode.Create);

        formatter.Serialize(stream, slotArray);
        stream.Close();
    }

    public static string[] LoadWorldSlots()
    {
        string loadPath = GameObject.Find("UIManager").GetComponent<UIManager>().appPath + "/saves/" + "/Beta 1.0/" + "/";

        if (File.Exists(loadPath + "saveData"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + "saveData", FileMode.Open);

            string[] saveSlots = formatter.Deserialize(stream) as string[];
            stream.Close();
            return saveSlots;
        }
        else
        {
            return null;
        }
    }
}
