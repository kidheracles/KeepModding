namespace ManagedMod.Generic;

using UndertaleModLib;

// If you use this, you have to have UndertaleModLib.dll and Underanalyzer.dll in your mods/aurie folder.

public static class UMT
{

    public static UndertaleData GetData(AurieManagedModule IModule, string? dataPath = null)
    {
        dataPath ??= Path.Combine(Framework.GetGameDirectory(), "data.win");
        if (File.Exists(dataPath))
        {
            using FileStream fs = File.OpenRead(dataPath);
            UndertaleData data = UndertaleIO.Read(fs);
            Framework.PrintEx(AurieLogSeverity.Trace, $"Successfully read data.win. Game Name: {data.GeneralInfo.DisplayName}");
            // You can now access various parts of the game data, e.g., data.Strings, data.Code, etc.
            return data;
        }
        else
        {
            throw new FileNotFoundException($"GMS data file not found at {dataPath}");
        }
    }
}