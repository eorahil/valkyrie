﻿using UnityEngine;
using System.Collections;

public class RtLFinder : AppFinder {
    // If the installed app isn't this or higher don't import
    override public string RequiredFFGVersion()
    {
        return "1.3.0";
    }
    // If an import wasn't performed with this Valkyrie version or higher reimport
    override public string RequiredValkyrieVersion()
    {
        return "0.2.3";
    }
    // Steam app ID
    override public string AppId()
    {
        return "477200";
    }
    // Where to store imported data
    override public string Destination()
    {
        return "D2E";
    }

    override public string DataDirectory()
    {
        return "/Road to Legend_Data";
    }
    override public string Executable()
    {
        return "Road to Legend.exe";
    }
    // RtL does not obfuscate text
    override public int ObfuscateKey()
    {
        return 0;
    }
}
