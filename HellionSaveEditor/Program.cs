using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HellionData;

namespace HellionSaveEditor
{
    class Program
    {
        private static JObject saveData;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // No arguments passed, look for most recent save in parent folder
                SaveFile.LoadLatestSaveFile(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            }
            else
            {
                SaveFile.LoadSaveFile(args[0]);
            }
            Console.WriteLine("Loaded save file: {0}", SaveFile.FilePath);

            MenuMain();

            Console.WriteLine();
            ConsoleColorLine("Would you like to save your changes? [o]verwrite, [b]ackup, [n]o {default}", ConsoleColor.DarkGreen);
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.O:
                    SaveFile.WriteSave(false);
                    break;
                case ConsoleKey.B:
                    SaveFile.WriteSave();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Prints out a message to the console using pretty colors.
        /// </summary>
        /// <param name="message">message to print out</param>
        /// <param name="bg">Optional Background color</param>
        /// <param name="fg">Optional Foreground color</param>
        private static void ConsoleColorLine(string message, ConsoleColor bg = ConsoleColor.Blue, ConsoleColor fg = ConsoleColor.Yellow)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;

            Console.WriteLine(message);

            Console.ResetColor();
        }

        private static void MenuMain()
        {
            while (true)
            {
                Console.WriteLine();
                ConsoleColorLine("]|I{------» Hellion Save Editor «------}I|[");
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


        #region Character Functions
        
        private static void MenuCharacter()
        {
            JObject characterJson = null;

            while (true)
            {
                Console.WriteLine();
                ConsoleColorLine("]|I{------» Character Editor «------}I|[");
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
                ConsoleColorLine("Character Found!", ConsoleColor.DarkGreen);
            }
            else
            {
                ConsoleColorLine("Character Not Found!!!", ConsoleColor.Red);
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
			Ship selectedShip = null;

            while (true)
            {
                Console.WriteLine();
                ConsoleColorLine("]|I{------» Ship Editor «------}I|[");
                Console.WriteLine();
                Console.WriteLine("1. List Ships");
                Console.WriteLine("2. Select My Ship");
                Console.WriteLine("3. Remove bad components from all ships");
				if (selectedShip != null)
                {
                    Console.WriteLine("A. Fill Resources");
                    Console.WriteLine("B. Fix Parts");
                    Console.WriteLine("C. Fix Room Air");
                    Console.WriteLine("D. Fix Entire Outpost (runs A,B,C,H to and Outpost ship and every child ship");
                    Console.WriteLine("E. Rename Ship");
                    Console.WriteLine("F. Remove bad components");
                    Console.WriteLine("G. Unlock doors");
                    Console.WriteLine("H. Fix Repair Points");
                }
                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

                switch (MenuResponse.Key)
                {
                    case ConsoleKey.D1:
                        ListShips();
                        break;
                    case ConsoleKey.D2:
						selectedShip = GetShip();
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine("What is the requested removal percentage?");
                        var systemRemovalPercentage = Convert.ToUInt16(Console.ReadLine());
                        //ShipRemoveBadComponents(25);
                        Ships.RemoveAllBadComponents(systemRemovalPercentage);
                        break;
					case ConsoleKey.A:
						//ShipResourceTanksFill (shipJson);
						selectedShip.ResourceTanksFill();
                        break;
					case ConsoleKey.B:
						//ShipDynamicObjectsFix(shipJson);
						selectedShip.DynamicObjectsFix();
                        break;
					case ConsoleKey.C:
						//ShipRoomsAir(shipJson);
						selectedShip.RoomsAirFill();
                        break;
                    case ConsoleKey.D:
                        //ShipOutpostFix(shipJson);
                        Ships.OutpostFix(selectedShip);
                        break;
                    case ConsoleKey.E:
                        Console.WriteLine("What would you like to rename your ship?");
                        string shipName = Console.ReadLine();
                        // ShipRename(shipJson, shipName);
						selectedShip.Name = shipName;
                        break;
                    case ConsoleKey.F:
                        Console.WriteLine("What is the requested removal percentage?");
                        var shipRemovalPercentage = Convert.ToUInt16(Console.ReadLine());
                        // ShipRemoveBadComponents(shipJson, removalPercentage);
						selectedShip.RemoveBadComponents(shipRemovalPercentage);
                        break;
                    case ConsoleKey.G:
                        // ShipDoorsUnlock(shipJson);
						selectedShip.DoorsUnlock();
                        break;
					case ConsoleKey.H:
						// ShipRepairPointsFix(shipJson);
						selectedShip.RepairPointsFix();
                        break;
                    case ConsoleKey.O:
                        Console.WriteLine("That's what she said, lol");
                        break;
                    case ConsoleKey.Q:
                        return;
                    default:
                        ConsoleColorLine(String.Format("The {0} key is not valid.", MenuResponse.Key), ConsoleColor.Red);
                        break;
                }
            }
        }

        /// <summary>
        /// List all the ships in the save file
        /// </summary>
        private static void ListShips()
        {
            ///Console.WriteLine();
            ///Console.WriteLine("Please enter a filter to limit results.");
            ///string shipFilter = Console.ReadLine();
            //TODO: Add the feature back to filter the ship list

			var ships = Ships.GetShips();

            foreach (var ship in ships)
            {
                Console.WriteLine(ship);
            }

			ConsoleColorLine (String.Format ("{0} ships found", ships.Count), ConsoleColor.Green);
        }

        /// <summary>
        /// Find a ship with a specific name and return it
        /// </summary>
        /// <returns></returns>
        private static Ship GetShip()
        {
            //TODO: Add the option to search by GUID
            Console.WriteLine();
            Console.WriteLine("Please type the name of the ship to select");
            string shipName = Console.ReadLine();

            Console.Write("Searching... ");

			var ship = Ships.GetShip(shipName);
            if (ship != null)
            {
                ConsoleColorLine(string.Format("Ship Found! {0}", ship), ConsoleColor.DarkGreen);
            }
            else
            {
                ConsoleColorLine("Ship Not Found!!!", ConsoleColor.Red);
            }
            return ship;
        }



        /// <summary>
        /// Change the name of your ship
        /// </summary>
        /// <param name="ship">The ship reference you wish to fix items for</param>
        /// <param name="shipName">What you want to rename you ship to</param>
        private static void ShipRename(JObject ship, string shipName)
        {
            Console.WriteLine();
            Console.WriteLine("Renaming ship {0} to {1}", ship["Name"], shipName);
            ship["Name"] = shipName;
        }

        #endregion
    }
}
