﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

// Class for getting lists of quest with details
// TODO: work out why this is so slow
public class QuestLoader {

    // Return a dictionary of all available quests
    public static Dictionary<string, Quest> GetQuests()
    {
        Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

        Game game = Game.Get();
        // Look in the user application data directory
        string dataLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Valkyrie";
        mkDir(dataLocation);
        // Get a list of quest directories (extract found packages)
        List<string> questDirectories = GetQuests(dataLocation);

        // If we are running inside unity paths are different
        if (Application.isEditor)
        {
            dataLocation = Application.dataPath + "/../quests/";
        }
        else
        {
            dataLocation = Application.dataPath + "/quests/";
        }
        // Add included quests (extract found packages)
        questDirectories.AddRange(GetQuests(dataLocation));

        // Add packaged quests that have been extracted
        questDirectories.AddRange(GetQuests(Path.GetTempPath() + "Valkyrie"));

        // Go through all directories
        foreach (string p in questDirectories)
        {
            // load quest
            Quest q = new Quest(p);
            // Check quest is valid and of the right type
            if (!q.name.Equals("") && q.type.Equals(Game.Get().gameType.TypeName()))
            {
                // Check that we have required expansions
                bool expansionsOK = true;
                foreach (string s in q.packs)
                {
                    if (!game.cd.GetEnabledPackIDs().Contains(s))
                    {
                        expansionsOK = false;
                    }
                }
                // Add quest to quest list
                if (expansionsOK) quests.Add(p, q);
            }
        }

        // Return list of available quests
        return quests;
    }

    // Return list of quests available in the user path (includes packages)
    public static Dictionary<string, Quest> GetUserQuests()
    {
        Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

        // Clean up extracted packages
        CleanTemp();

        // Read user application data for quests
        string dataLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Valkyrie";
        mkDir(dataLocation);
        List<string> questDirectories = GetQuests(dataLocation);

        // Read extracted packages
        questDirectories.AddRange(GetQuests(Path.GetTempPath() + "Valkyrie"));

        // go through all found quests
        foreach (string p in questDirectories)
        {
            // read quest
            Quest q = new Quest(p);
            // Check if valid and correct type
            if (!q.name.Equals("") && q.type.Equals(Game.Get().gameType.TypeName()))
            {
                quests.Add(p, q);
            }
        }

        return quests;
    }

    // Return list of quests available in the user path unpackaged (editable)
    public static Dictionary<string, Quest> GetUserUnpackedQuests()
    {
        Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

        // Read user application data for quests
        string dataLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Valkyrie";
        mkDir(dataLocation);
        List<string> questDirectories = GetQuests(dataLocation);

        // go through all found quests
        foreach (string p in questDirectories)
        {
            // read quest
            Quest q = new Quest(p);
            // Check if valid and correct type
            if (!q.name.Equals("") && q.type.Equals(Game.Get().gameType.TypeName()))
            {
                quests.Add(p, q);
            }
        }

        return quests;
    }

    // Get list of directories with quests at a path, and extract found packages
    public static List<string> GetQuests(string path)
    {
        List<string> quests = new List<string>();

        if (!Directory.Exists(path))
        {
            return quests;
        }

        // Get all directories at path
        List<string> questDirectories = DirList(path);
        foreach (string p in questDirectories)
        {
            // All packs must have a quest.ini, otherwise ignore
            if (File.Exists(p + "/quest.ini"))
            {
                    quests.Add(p);
            }
        }

        // Find all packages at path
        string[] archives = Directory.GetFiles(path, "*.valkyrie", SearchOption.AllDirectories);
        // Extract all packages
        foreach (string f in archives)
        {
            // Extract into temp
            mkDir(Path.GetTempPath() + "/Valkyrie");
            string extractedPath = Path.GetTempPath() + "Valkyrie/" + Path.GetFileName(f);
            if (Directory.Exists(extractedPath))
            {
                try
                {
                    Directory.Delete(extractedPath, true);
                }
                catch (System.Exception)
                {
                    Debug.Log("Warning: Unable to remove old temporary files: " + extractedPath);
                }
            }
            mkDir(extractedPath);

            try
            {
                ZipFile zip = ZipFile.Read(f);
                zip.ExtractAll(extractedPath);
            }
            catch (System.Exception)
            {
                Debug.Log("Warning: Unable to read file: " + extractedPath);
            }
        }

        return quests;
    }

    // Attempt to create a directory
    public static void mkDir(string p)
    {
        if (!Directory.Exists(p))
        {
            try
            {
                Directory.CreateDirectory(p);
            }
            catch (System.Exception)
            {
                Debug.Log("Error: Unable to create directory: " + p);
                Application.Quit();
            }
        }
    }

    // Return a list of directories at a path (recursive)
    public static List<string> DirList(string path)
    {
        return DirList(path, new List<string>());
    }

    // Add to list of directories at a path (recursive)
    public static List<string> DirList(string path, List<string> l)
    {
        List<string> list = new List<string>(l);

        foreach (string s in Directory.GetDirectories(path))
        {
            list = DirList(s, list);
            list.Add(s);
        }

        return list;
    }

    public static void CleanTemp()
    {
        // Nothing to do if no temporary files
        if (!Directory.Exists(Path.GetTempPath() + "/Valkyrie"))
        {
            return;
        }

        try
        {
            Directory.Delete(Path.GetTempPath() + "/Valkyrie", true);
        }
        catch (System.Exception)
        {
            Debug.Log("Warning: Unable to remove temporary files.");
        }
    }

    // Read quest for for validity
    public class Quest
    {
        public string path;
        public string name = "";
        public string description;
        public string type;
        public string[] packs;

        public Quest(string p)
        {
            path = p;
            IniData d = IniRead.ReadFromIni(p + "/quest.ini");
            if (d == null)
            {
                Debug.Log("Warning: Invalid quest:" + p + "/quest.ini!");
                return;
            }

            type = d.Get("Quest", "type");
            if (type.Length == 0)
            {
                // Default to D2E to support historical quests
                type = "D2E";
            }

            name = d.Get("Quest", "name");
            if (name.Equals(""))
            {
                Debug.Log("Warning: Failed to get name data out of " + p + "/content_pack.ini!");
                return;
            }

            if (d.Get("Quest", "packs").Length > 0)
            {
                packs = d.Get("Quest", "packs").Split(' ');
            }
            else
            {
                packs = new string[0];
            }

            // Missing description is OK
            description = d.Get("Quest", "description");
        }
    }
}
