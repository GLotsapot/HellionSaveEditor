using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionData
{
    /// <summary>
    /// Worker class for retrieving ship object from raw json
    /// </summary>
    public static class Ships
    {
        /// <summary>
        /// Get a ship based upon it's GUID
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns>A ship if found, or null if not</returns>
        public static Ship GetShip(UInt64 GUID)
        {
            if (SaveFile.IsLoaded)
            {
                JObject ship = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["GUID"].Value<string>() == GUID.ToString()).First();
                if (ship != null)
                {
                    return new Ship(ship);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve ship GUID {0}", GUID);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Get a ship based upon it's name or registration
        /// </summary>
        /// <param name="NameOrRegistration"></param>
        /// <returns>A ship if found, or null if not</returns>
		public static Ship GetShip(string NameOrRegistration)
		{
			if (SaveFile.IsLoaded)
			{
                JObject ship = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["Registration"].Value<string>() == NameOrRegistration.ToUpper() || o["Name"].Value<string>() == NameOrRegistration.ToUpper()).FirstOrDefault();
                if(ship != null)
                {
                    return new Ship(ship);
                }
                else
                {
                    return null;
                }
			}
			else
			{
				string message = String.Format("Save file not loaded. Cannot retrieve ship with name or registration of {0}", NameOrRegistration);
				throw new Exception(message);
			}			

		}

        /// <summary>
        /// Get a list of ships that are docked to the parentShip
        /// </summary>
        /// <param name="parentShip"></param>
        /// <returns></returns>
        public static List<Ship> GetShipChildren(Ship parentShip)
        {
            if (SaveFile.IsLoaded)
            {
                IEnumerable<JObject> shipChildren = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"] != null).Where(o => o["DockedToShipGUID"].Value<ulong>() == parentShip.GUID);

                // TODO: Maybe thread this to improve retrieval?
                // Loop through and return all ship objects. 
                List<Ship> returnShips = new List<Ship>();
                foreach (JObject ship in shipChildren)
                {
                    returnShips.Add(new Ship(ship));
                }

                return returnShips;
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve children of ship GUID {0}", parentShip.GUID);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Gets all ships from the save. Optionally, just get a list of ships that don't have parent ships
        /// </summary>
        /// <param name="parentsOnly">If you would like to get ships that aren't docked to another ship</param>
        /// <returns></returns>
        public static List<Ship> GetShips(Boolean parentsOnly = false)
        {
            if (!SaveFile.IsLoaded)
            {
				string message = String.Format("Save file not loaded. Cannot retrieve ships");
				throw new Exception(message);
            }

			IEnumerable<JObject> shipsJson;
			if (!parentsOnly) {
				shipsJson = SaveFile.saveData["Ships"].Children<JObject>();
			} else {
				shipsJson = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"] != null);
			}

			List<Ship> ships = new List<Ship>();
			foreach (var shipJson in shipsJson)
			{
				ships.Add (new Ship (shipJson));
			}

			return ships;
        }

        /// <summary>
        /// Removes bad components from ALL ships
        /// </summary>
        /// <param name="removalPercentage"></param>
        public static void RemoveAllBadComponents(uint removalPercentage)
        {
            var ships = Ships.GetShips();
            foreach (Ship ship in ships)
            {
                ship.RemoveBadComponents(removalPercentage);
            }
        }

        /// <summary>
        /// Fixes all systems on the ship (Objects, Repair points, ResourceTanks, Air)
        /// </summary>
        /// <param name="ship">The ship to fix</param>
        public static void ShipFix(Ship ship)
        {
            //TODO: Move this to Ship class where it belongs
            ship.DynamicObjectsFix();
            ship.RepairPointsFix();
            ship.ResourceTanksFill();
            ship.RoomsAirFill();
        }

        /// <summary>
        /// Fix the parentShip, and every ship in the tree below it recursivly
        /// </summary>
        /// <param name="parentShip">The top most ship to start with</param>
        public static void OutpostFix(Ship parentShip)
        {
            ShipFix(parentShip);

            var children = Ships.GetShipChildren(parentShip);
            foreach (Ship ship in children)
            {
                Ships.OutpostFix(ship);
            }
        }

        /// <summary>
        /// Finds the top most parent ship in the tree of connected ships
        /// </summary>
        /// <param name="startingShip">The initial ship to start the search from</param>
        /// <returns>The ship that the startingShip originated from</returns>
        public static Ship FindMasterShip(Ship startingShip)
        {
            Ship currentShip = startingShip;
            while (currentShip.DockedToShipGUID != 0)
            {
                currentShip = currentShip.GetParentShip();
            }

            return currentShip;
        }
    }
}
