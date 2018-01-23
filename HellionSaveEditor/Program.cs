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
        static void Main(string[] args)
        {
			try {
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
				
			} catch (Exception ex) {
				ConsoleColorLine ("Oh no! There was an error trying to load a save file! Cannot continue.", ConsoleColor.Yellow, ConsoleColor.Red);
				Console.WriteLine (ex);
				return;
			}
            

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
            Player selectedPlayer = null;

            while (true)
            {
                Console.WriteLine();
                ConsoleColorLine("]|I{------» Character Editor «------}I|[");
                Console.WriteLine();
                Console.WriteLine("1. List Character Names");
                Console.WriteLine("2. Select My Character");
                if(selectedPlayer != null)
                {
                    Console.WriteLine("3. Fill Health");
                    Console.WriteLine("4. Put on Space Suit");
                    Console.WriteLine("5. Rename Character");
                }

                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

				try {
	                switch (MenuResponse.Key)
	                {
	                    case ConsoleKey.D1:
	                        CharacterList();
	                        break;
	                    case ConsoleKey.D2:
	                        selectedPlayer = CharacterSelect();
	                        break;
	                    case ConsoleKey.D3:
	                        CharacterFillHealth(selectedPlayer);
	                        break;
	                    case ConsoleKey.D4:
	                        CharacterSuitUp(selectedPlayer);
	                        break;
	                    case ConsoleKey.D5:
	                        CharacterRename(selectedPlayer);
	                        break;
	                    case ConsoleKey.Q:
	                        return;

	                }
				} catch (Exception ex) {
					ConsoleColorLine ("Oh no! There was an error trying to do that. Please provide the below error, as well what you were trying to do to the developers through either the STEAM forum, or (preferably) through GitHub bugs", ConsoleColor.Red);
					Console.WriteLine (ex);
				}
            }
        }

        private static void CharacterList()
        {
            var characters = Players.GetPlayers();

            Console.WriteLine();
            foreach (var character in characters)
            {
                Console.WriteLine(String.Format("[{0}] {1}", character.GUID, character.Name));
            }

        }

        private static Player CharacterSelect()
        {
            Console.WriteLine();
			Console.WriteLine("Please type the GUID of the character to select");
			long characterGUID;
			try
			{
				characterGUID = Int64.Parse(Console.ReadLine());
			}
			catch (Exception ex)
			{
				var newEx = new InvalidCastException("The player GUID you tried to search was not a number. Seach failed.", ex);
				throw newEx;
			}


			Player player = Players.GetPlayer(characterGUID);
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

        private static void CharacterFillHealth(Player currentPlayer)
        {
			Console.WriteLine("Health is currently: {0}", currentPlayer.HealthPoints);
            currentPlayer.FillHealth();
            ConsoleColorLine("Character health filled", ConsoleColor.DarkGreen);
        }

        private static void CharacterRename(Player currentPlayer)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to rename this character?");
            string characterName = Console.ReadLine();

            currentPlayer.Name = characterName;
            ConsoleColorLine("Character renamed", ConsoleColor.DarkGreen);
        }

        private static void CharacterSuitUp(Player currentPlayer)
        {
            Console.WriteLine();
            currentPlayer.GiveSuit();

            ConsoleColorLine("Character suited up.", ConsoleColor.DarkGreen);
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
                    Console.WriteLine("D. Fix Ship and Children (runs A,B,C,H on the current ship, and all it's recursive children");
                    Console.WriteLine("E. Rename Ship");
                    Console.WriteLine("F. Remove bad components");
                    Console.WriteLine("G. Unlock doors");
                    Console.WriteLine("H. Fix Repair Points");
                    Console.WriteLine("I. Select Master Ship (Top most parent ship in the Outpost");
                }
                Console.WriteLine("Q. Quit.");
                var MenuResponse = Console.ReadKey(true);

				try {
					
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
	                        Ships.RemoveAllBadComponents(systemRemovalPercentage);
	                        break;
						case ConsoleKey.A:
							selectedShip.ResourceTanksFill();
	                        break;
						case ConsoleKey.B:
							selectedShip.DynamicObjectsFix();
	                        break;
						case ConsoleKey.C:
							selectedShip.RoomsAirFill();
	                        break;
	                    case ConsoleKey.D:
	                        Ships.OutpostFix(selectedShip);
	                        break;
	                    case ConsoleKey.E:
	                        Console.WriteLine("What would you like to rename your ship?");
	                        string shipName = Console.ReadLine();
							selectedShip.Name = shipName;
	                        break;
	                    case ConsoleKey.F:
	                        Console.WriteLine("What is the requested removal percentage?");
	                        var shipRemovalPercentage = Convert.ToUInt16(Console.ReadLine());
							selectedShip.RemoveBadComponents(shipRemovalPercentage);
	                        break;
	                    case ConsoleKey.G:
							selectedShip.DoorsUnlock();
	                        break;
						case ConsoleKey.H:
							selectedShip.RepairPointsFix();
	                        break;
	                    case ConsoleKey.I:
	                        selectedShip = Ships.FindMasterShip(selectedShip);
	                        ConsoleColorLine(String.Format("Ship Found! {0}", selectedShip), ConsoleColor.DarkGreen);
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
				} catch (Exception ex) {
					ConsoleColorLine ("Oh no! There was an error trying to do that. Please provide the below error, as well what you were trying to do to the developers through either the STEAM forum, or (preferably) through GitHub bugs", ConsoleColor.Yellow, ConsoleColor.Red);
					Console.WriteLine (ex);
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

			ConsoleColorLine (String.Format ("{0} ships found", ships.Count), ConsoleColor.DarkGreen);
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
