using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HellionSaveEditor
{
    class Program
    {
        private static JObject saveData;

        static void Main(string[] args)
        {
            string saveFileName = LoadLastSave(@"..\");

            MenuMain();

            Console.WriteLine();
            Console.WriteLine("Would you like to save your changes? (y/n)");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    WriteSave(saveFileName);
                    break;
                default:
                    break;
            }
        }

        private static void MenuMain()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("]|I{------» Hellion Save Editor «------}I|[");
                Console.WriteLine();
                Console.WriteLine("1. Character Edit");
                Console.WriteLine("2. Ship Edit");

                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

                switch (MenuResponse.Key)
                {
                    case System.ConsoleKey.D1:
                        MenuCharacter();
                        break;
                    case System.ConsoleKey.D2:
                        MenuShip();
                        break;
                    case ConsoleKey.Q:
                        return;

                }
            }
        }


        #region File Functions

        /// <summary>
        /// Finds the *.save game with the most recent Date Modified.
        /// This is usually the latest save game, but can be used to force a specific save to load instead
        /// </summary>
        /// <param name="savePath"></param>
        /// <returns>The path fo find the save games</returns>
        private static string LoadLastSave(string savePath)
        {
            var files = new System.IO.DirectoryInfo(savePath).GetFileSystemInfos("*.save").OrderBy(f => f.LastWriteTime);

            if(files.Count() != 0)
            {
                LoadSave(files.Last().FullName);
                return files.Last().FullName;
            }
            else
            {
                Console.WriteLine("Could not find any save games to edit.");
                return null;
            }
            
        }

        /// <summary>
        /// Opens the file specified and loads the JSon data to be manupulated
        /// </summary>
        /// <param name="saveFile">The save file to load data from</param>
        private static void LoadSave(string saveFile)
        {
            Console.WriteLine("Reading save file: {0}", saveFile);
            using (StreamReader file = File.OpenText(saveFile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    saveData = (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        /// <summary>
        /// Renames the original file for backup, and saves the new file in it's place
        /// </summary>
        /// <param name="saveFile">The full save path and name to save data to<</param>
        private static void WriteSave(string saveFile)
        {
            System.IO.File.Move(saveFile, saveFile + ".backup");
            using (StreamWriter sWriter = File.CreateText(saveFile))
            {
                using (JsonTextWriter jWriter = new JsonTextWriter(sWriter))
                {
                    jWriter.Formatting = Formatting.Indented;
                    //jWriter.Indentation = 4;

                    saveData.WriteTo(jWriter);
                }
            }

        }

        #endregion

        #region Character Functions
        
        private static void MenuCharacter()
        {
            JObject characterJson = null;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("]|I{------» Character Editor «------}I|[");
                Console.WriteLine();
                Console.WriteLine("1. List Character Names");
                Console.WriteLine("2. Select My Character");
                if(characterJson != null)
                {
                    Console.WriteLine("3. Fill Health");
                }

                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

                switch (MenuResponse.Key)
                {
                    case ConsoleKey.D1:
                        CharacterList();
                        break;
                    case ConsoleKey.D2:
                        characterJson = CharacterSelect();
                        break;
                    case ConsoleKey.D3:
                        if(characterJson != null)
                        {
                            CharacterFillHealth(characterJson);
                        }
                        else
                        {
                            Console.WriteLine("Character has not been selected yet.");
                        }
                        break;
                    case ConsoleKey.Q:
                        return;

                }
            }
        }

        private static void CharacterList()
        {
            var characters = saveData["Players"].Children<JObject>();

            Console.WriteLine();
            foreach (var character in characters)
            {
                Console.WriteLine(character["Name"]);
            }

        }

        private static JObject CharacterSelect()
        {
            Console.WriteLine();
            Console.WriteLine("Please type the name of the character to select");
            string characterName = Console.ReadLine();

            Console.Write("Searching... ");

            JObject player = saveData["Players"].Children<JObject>().FirstOrDefault(o => o["Name"].ToString() == characterName);
            if(player != null)
            {
                Console.WriteLine("Character Found!");
            }
            else
            {
                Console.WriteLine("Character Not Found!!!");
            }
            return player;
        }

        private static void CharacterFillHealth(JObject character)
        {
            Console.WriteLine();
            Console.WriteLine("Character health was at: {0}", character["HealthPoints"]);
            character["HealthPoints"] = character["MaxHealthPoints"];
            Console.WriteLine("Character health changed to: {0}", character["MaxHealthPoints"]);
        }

        #endregion

        #region Ship Functions

        private static void MenuShip()
        {
            JObject shipJson = null;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("]|I{------» Ship Editor «------}I|[");
                Console.WriteLine();
                Console.WriteLine("1. List Ships");
                Console.WriteLine("2. Select My Ship");
                if(shipJson != null)
                {
                    Console.WriteLine("3. Fill Resources");
                    Console.WriteLine("4. Fix Parts");
                    Console.WriteLine("5. Fix Room Air");
                    Console.WriteLine("6. Fix Entire Outpost (runs 3,4,5 to and Outpost ship and every child ship");
                }
                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

                switch (MenuResponse.Key)
                {
                    case ConsoleKey.D1:
                        ListShips();
                        break;
                    case ConsoleKey.D2:
                        shipJson = GetShip();
                        break;
                    case ConsoleKey.D3:
                        ShipResourceTanksFill(shipJson);
                        break;
                    case ConsoleKey.D4:
                        ShipDynamicObjectsFix(shipJson);
                        break;
                    case ConsoleKey.D5:
                        ShipRoomsAir(shipJson);
                        break;
                    case ConsoleKey.D6:
                        ShipOutpostFix(shipJson);
                        break;
                    case ConsoleKey.Q:
                        return;
                }
            }
        }

        private static void ListShips()
        {
            Console.WriteLine();
            Console.WriteLine("Please enter a filter to limit results.");
            string shipFilter = Console.ReadLine();

            //var ships = saveData["Ships"].Children<JObject>().Where(n => n["Name"].Value<string>() == shipFilter);

            var ships = from s in saveData["Ships"]
                        where s["Name"].Value<string>().Contains(shipFilter)
                        select s;

            foreach (var ship in ships)
            {
                Console.WriteLine(ship["Name"].Value<string>());
            }
        }

        private static JObject GetShip()
        {
            Console.WriteLine();
            Console.WriteLine("Please type the name of the ship to select");
            string shipName = Console.ReadLine();

            Console.Write("Searching... ");

            JObject ship = saveData["Ships"].Children<JObject>().FirstOrDefault(o => o["Name"].ToString() == shipName);
            if (ship != null)
            {
                Console.WriteLine("Ship Found! GUID: {0}", ship["GUID"].Value<string>());
            }
            else
            {
                Console.WriteLine("Ship Not Found!!!");
            }
            return ship;
        }

        /// <summary>
        /// This will fix the air pressure, air quality, parts, and resourcs tanks of all connected objects.
        /// </summary>
        /// <param name="parentShip">The parent ship that everything attaches to. Usually an Outpost.</param>
        private static void ShipOutpostFix(JObject parentShip)
        {
            Console.WriteLine("-- Fixing Room {0} --", parentShip["Name"].Value<string>());
            
            ShipRoomsAir(parentShip);
            ShipResourceTanksFill(parentShip);
            ShipDynamicObjectsFix(parentShip);

            // Get ships that are docked to this ship, and repeat
            var childrenShips = saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"].Value<string>() == parentShip["GUID"].Value<string>());
            foreach (JObject childShip in childrenShips) { ShipOutpostFix(childShip); }
        }

        /// <summary>
        /// Goes through each Dynamic Objects that has PartData, and changes it's Health to match MaxHealth
        /// </summary>
        /// <param name="ship">The ship reference you wish to fix items for</param>
        private static void ShipDynamicObjectsFix(JObject ship)
        {
            // throw new NotImplementedException();

            var partObjects = from po in ship["DynamicObjects"]
                              where po["PartData"] != null
                              select po;

            foreach (var po in partObjects)
            {
                Console.WriteLine("- {0}: {1}", po["GUID"], po["PartData"]["Health"]);
                po["PartData"]["Health"] = po["PartData"]["MaxHealth"];
            }
        }

        /// <summary>
        /// Fills each resource tank in the ship to it's max capacity
        /// </summary>
        /// <param name="ship">The ship reference you wish to fill</param>
        private static void ShipResourceTanksFill(JObject ship)
        {
            Console.WriteLine();
            Console.WriteLine("Filling the tanks");

            foreach (var resourceTank in ship["ResourceTanks"].Children<JObject>())
            {
                Console.WriteLine("- {0}: {1}", resourceTank["CargoCompartments"][0]["Name"], resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"]);
                var capacity = resourceTank["CargoCompartments"][0]["Capacity"];
                resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"] = capacity;
            }
        }

        /// <summary>
        /// Sets the pressure and quality of each room to 1.0
        /// </summary>
        /// <param name="ship">The ship reference you wish to fill</param>
        private static void ShipRoomsAir(JObject ship)
        {
            Console.WriteLine();
            foreach (var room in ship["Rooms"].Children<JObject>())
            {
                Console.WriteLine("Changeing Room {0}", room["GUID"].Value<string>());
                Console.WriteLine("- AP:{0} AQ:{1}", room["AirPressure"], room["AirQuality"]);
                room["AirPressure"] = 1.0;
                room["AirQuality"] = 1.0;
            }
        }

        #endregion
    }
}
