using Godot;
using System;
using System.Linq;

public partial class Gamedata : Node
{
	private const string SavePath = "user://savegame.save";

    public struct SaveData
    {
        public int Money;
        public int SelectedLanguage;
    }

    public static void SaveGame( int currentMoney, int SelectedLanguage)
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null) return;
        var data = new Godot.Collections.Dictionary<string, int>
        {
            {
                "Money", currentMoney},
                {"Language", SelectedLanguage}
        };
        string jsonString = Json.Stringify(data);
        file.StoreString(jsonString);
        GD.Print("Game Opgeslagen: " + jsonString);
    }

    public static SaveData LoadGame()
    {
        SaveData loadedData = new SaveData { Money = 1000, SelectedLanguage = 0 };

        if (!FileAccess.FileExists(SavePath)) return loadedData;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();
        
        var json = new Json();
        var parseResult = json.Parse(jsonString);

        if (parseResult == Error.Ok)
        {
            var data = (Godot.Collections.Dictionary)json.Data;
            
            if (data.ContainsKey("Money")) 
                loadedData.Money = (int)data["Money"];
            
            if (data.ContainsKey("Language")) 
                loadedData.SelectedLanguage = (int)data["Language"];
        }

        return loadedData;;
    }
}
