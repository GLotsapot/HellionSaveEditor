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
						selectedShip = GetShip(); //TODO: reference the ship to edit
                        break;
                    case ConsoleKey.D3:
                        //TODO: remove all bad components
                        //ShipRemoveBadComponents(25);
                        Ships.RemoveAllBadComponents(25);
                        break;
					case ConsoleKey.A:
						//ShipResourceTanksFill (shipJson);
						selectedShip.ResourceTanksFill();
                        break;
					case ConsoleKey.B:
						//ShipDynamicObjectsFix(shipJson);
						selectedShip.DynamicObjectsFix ();
                        break;
					case ConsoleKey.C:
						//ShipRoomsAir(shipJson);
						selectedShip.RoomsAirFill ();
                        break;
                    case ConsoleKey.D:
                        //ShipOutpostFix(shipJson);
						//TODO: Write a new Outpost Fix
						throw new NotImplementedException();
                        break;
                    case ConsoleKey.E:
                        Console.WriteLine("What would you like to rename your ship?");
                        string shipName = Console.ReadLine();
                        // ShipRename(shipJson, shipName);
						selectedShip.Name = shipName;
                        break;
                    case ConsoleKey.F:
                        Console.WriteLine("What is the requested removal percentage?");
                        var removalPercentage = Convert.ToInt16(Console.ReadLine()) / 100.0;
                        // ShipRemoveBadComponents(shipJson, removalPercentage);
						selectedShip.RemoveBadComponents(removalPercentage);
                        break;
                    case ConsoleKey.G:
                        // ShipDoorsUnlock(shipJson);
						selectedShip.DoorsUnlock();
                        break;
					case ConsoleKey.H:
						// ShipRepairPointsFix (shipJson);
						selectedShip.RepairPointsFix();
                        break;
                    case ConsoleKey.Q:
                        return;
                    default:
                        ConsoleColorLine(string.Format("The {0} key is not valid.", MenuResponse.Key), ConsoleColor.Red);
                        break;
                }
            }
        }

        private static void ListShips()
        {
            ///Console.WriteLine();
            ///Console.WriteLine("Please enter a filter to limit results.");
            ///string shipFilter = Console.ReadLine();

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
        /// Remove components from a ship if it's health is below a percentage
        /// </summary>
        /// <param name="ship">The ship reference you wish to fix items for</param>
        /// <param name="removalPercentage">The decimal percentage barrier to remove</param>
        private static void ShipRemoveBadComponents(JObject ship, double removalPercentage)
        {
            if (removalPercentage > 1)
            {
                ConsoleColorLine("Percentage cannot be above 100%", ConsoleColor.Red);
                return;
            }
            else
            {
                Console.WriteLine("Removing parts with health lower than {0:P} from {1}", removalPercentage, ship["Name"]);
            }

            var partObjects = from po in ship["DynamicObjects"]
                              where po["PartData"] != null
                              select po;

            
            // NOTE: We have to do this in reverse, as doing it foward and removing causes an Exception
            foreach (var po in partObjects.Reverse())
            {
                double partHealth = po["PartData"]["Health"].Value<double>() / po["PartData"]["MaxHealth"].Value<double>();
                Console.WriteLine("- Part {0}: Health {1:P}", po["GUID"], partHealth);

                if (partHealth < removalPercentage)
                {
                    Console.WriteLine("-- Removing Part");
                    po.Remove();
                }
                else
                {
                    Console.WriteLine("-- Keeping Part");
                }
            }
        }

        /// <summary>
        /// Remove components from all ships if it's health is below a percentage
        /// </summary>
        /// <param name="removalPercentage">The decimal percentage barrier to remove</param>
        private static void ShipRemoveBadComponents(double removalPercentage)
        {
            Console.WriteLine("Removing parts with health lower than {0:P} from all ships", removalPercentage);

            var ships = saveData["Ships"].Children<JObject>();

            foreach (var ship in ships)
            {
                ShipRemoveBadComponents(ship, removalPercentage);
            }
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

        /// <summary>
        /// This will fix the air pressure, air quality, parts, and resourcs tanks of all connected objects.
        /// </summary>
        /// <param name="parentShip">The parent ship that everything attaches to. Usually an Outpost.</param>
        private static void ShipOutpostFix(JObject parentShip)
        {
            Console.WriteLine();
            ConsoleColorLine(string.Format("-- Fixing Room {0} ({0}) --", parentShip["Registration"].Value<string>(), parentShip["Name"].Value<string>()), ConsoleColor.DarkGreen);
            
            ShipRoomsAir(parentShip);
            ShipResourceTanksFill(parentShip);
            ShipDynamicObjectsFix(parentShip);
            ShipRepairPointsFix(parentShip);

            // Get ships that are docked to this ship, and repeat
            var childrenShips = saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"] != null).Where(o => o["DockedToShipGUID"].Value<Int64>() == parentShip["GUID"].Value<Int64>());

            foreach (JObject childShip in childrenShips) { ShipOutpostFix(childShip); }
        }

        /// <summary>
        /// Goes through each Dynamic Objects that has PartData, and changes it's Health to match MaxHealth
        /// </summary>
        /// <param name="ship">The ship reference you wish to fix items for</param>
        private static void ShipDynamicObjectsFix(JObject ship)
        {
            Console.WriteLine();
            ConsoleColorLine("Fixing Dynamic Objects", ConsoleColor.DarkBlue);

            var partObjects = from po in ship["DynamicObjects"]
                              where po["PartData"] != null
                              select po;

            foreach (var po in partObjects)
            {
                Console.WriteLine("- GUID {0}: {1}", po["GUID"], po["PartData"]["Health"]);
                if (po["PartData"]["Health"].Value<double>() != po["PartData"]["MaxHealth"].Value<double>())
                {
                    Console.WriteLine("-- Changing health to {0}", po["PartData"]["MaxHealth"]);
                    po["PartData"]["Health"] = po["PartData"]["MaxHealth"];
                }
            }
        }

        /// <summary>
        /// Fills each resource tank in the ship to it's max capacity
        /// </summary>
        /// <param name="ship">The ship reference you wish to fill</param>
        private static void ShipResourceTanksFill(JObject ship)
        {
            Console.WriteLine();
            ConsoleColorLine("Filling Resource Tanks", ConsoleColor.DarkBlue);

            foreach (var resourceTank in ship["ResourceTanks"].Children<JObject>())
            {
                Console.WriteLine("- {0}: {1}", resourceTank["CargoCompartments"][0]["Name"], resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"]);

                if(resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"].Value<double>() != resourceTank["CargoCompartments"][0]["Capacity"].Value<double>())
                {
                    var capacity = resourceTank["CargoCompartments"][0]["Capacity"];

                    Console.WriteLine("-- Changing quantity to {0}", capacity);
                    resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"] = capacity;
                }
            }
        }

        /// <summary>
        /// Sets the pressure and quality of each room to 1.0
        /// </summary>
        /// <param name="ship">The ship reference you wish to fill</param>
        private static void ShipRoomsAir(JObject ship)
        {
            Console.WriteLine();
            ConsoleColorLine("Fixing Rooms Atmosphere", ConsoleColor.DarkBlue);

            foreach (var room in ship["Rooms"].Children<JObject>())
            {
                Console.WriteLine("Changeing Room {0}", room["GUID"].Value<string>());
                if(room["AirPressure"].Value<double>() != 1)
                {
                    Console.WriteLine("- AP changed from {0}", room["AirPressure"]);
                    room["AirPressure"] = 1.0;
                }

                if (room["AirQuality"].Value<double>() != 1)
                {
                    Console.WriteLine("- AQ changed from {0}", room["AirQuality"]);
                    room["AirQuality"] = 1.0;
                }
            }
        }

        /// <summary>
        /// Sets all the doors on the ship to unlocked
        /// </summary>
        /// <param name="ship">The ship reference you wish to unlock doors on</param>
        private static void ShipDoorsUnlock(JObject ship)
        {
            Console.WriteLine();
            ConsoleColorLine("Unlocking doors", ConsoleColor.DarkBlue);
            ConsoleColorLine(" - Currently not implemented", ConsoleColor.Red);
            return;

            foreach (var door in ship["Doors"].Children<JObject>().Where(o => o["IsLocked"].Value<bool>() == true))
            {
                Console.WriteLine("Unlocking door with skeleton key");
                door["IsLocked"] = false;
            }
        }

        /// <summary>
        /// Goes through each Repair Point and changes it's Health to match MaxHealth
        /// </summary>
        /// <param name="ship">The ship reference you wish to unlock doors on</param>
        private static void ShipRepairPointsFix(JObject ship)
        {
            Console.WriteLine();
            ConsoleColorLine("Fixing Repair Points", ConsoleColor.DarkBlue);

            var rpObjects = from rp in ship["RepairPoints"]
                            select rp;

            int totalHealth = 0;
            foreach(var rp in rpObjects)
            {
                Console.WriteLine("- Changing {0} to {1}", rp["Health"], rp["MaxHealth"]);
                rp["Health"] = rp["MaxHealth"];
                totalHealth = totalHealth + rp["MaxHealth"].ToObject<int>();
            }

            ship["Health"] = totalHealth;
            Console.WriteLine("-- Total ship health changed to {0}", totalHealth);
        }

        #endregion
    }
}
