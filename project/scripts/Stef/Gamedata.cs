using Godot;
using System;

public partial class Gamedata : Node
{
	private const string SavePath = "user://savegame.save";

    public static void SaveGame(int currentMoney)
    {
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null) return;
        var data = new Godot.Collections.Dictionary<string, int>
        {
            { "Money", currentMoney }
        };
        string jsonString = Json.Stringify(data);
        file.StoreString(jsonString);
        GD.Print("Game Opgeslagen: " + jsonString);
    }

    public static int LoadGame()
    {
        if (!FileAccess.FileExists(SavePath)) return 0;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();
        
        var json = new Json();
        var parseResult = json.Parse(jsonString);

        if (parseResult == Error.Ok)
        {
            var data = new Godot.Collections.Dictionary<string, int>((Godot.Collections.Dictionary)json.Data);
            return data.ContainsKey("Money") ? data["Money"] : 0;
        }

        return 0;
    }
}
