using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using SoR.Logic.Character;

namespace SoR.Hardware.Data
{
    /*
     * Save game data using JSON serialisation.
     */
    public class GameState
    {
        public Vector2 Position { get; set; }
        public int HitPoints { get; set; }
        public string Skin { get; set; }
        public string Name { get; set; }
        public string CurrentMap { get; set; }

        /*
         * Save the current game state to a JSON file.
         */
        public static void SaveFile(Entity player, string playerLocation)
        {
            GameState save = new()
            {
                Position = player.GetPosition(),
                HitPoints = player.HitPoints,
                Skin = player.Skin,
                Name = player.Name,
                CurrentMap = playerLocation
            };

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto
            };

            string jsonData = JsonConvert.SerializeObject(save, Formatting.Indented, jsonSettings);

            Directory.CreateDirectory(Globals.GetSavePath("SoR"));

            string filePath = Globals.GetSavePath("SoR\\saveFile.json");

            File.WriteAllText(filePath, jsonData);
        }

        /*
         * Load saved game data from a JSON file.
         */
        public static GameState LoadFile()
        {
            string filePath = Globals.GetSavePath("SoR\\saveFile.json");

            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);

                    GameState loadedGameData = JsonConvert.DeserializeObject<GameState>(jsonData);

                    return loadedGameData;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error encountered while attempting to load game data: {ex.Message}");
            }

            return null;
        }
    }
}