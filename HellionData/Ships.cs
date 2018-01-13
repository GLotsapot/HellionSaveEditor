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

        public static List<Ship> GetShipChildren(Ship parentShip)
        {
            if (SaveFile.IsLoaded)
            {
                IEnumerable<JObject> shipChildren = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"].Value<ulong>() == parentShip.GUID);

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

        public static void RemoveAllBadComponents(double removalPercentage)
        {
            throw new NotImplementedException();
        }

        public static void OutpostFix(Ship parentShip)
        {
            parentShip.DynamicObjectsFix();
            parentShip.RepairPointsFix();
            parentShip.ResourceTanksFill();
            parentShip.RoomsAirFill();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the top most parent ship in the tree of connected ships
        /// </summary>
        /// <param name="startingShip">The initial ship to start the search from</param>
        /// <returns>The ship that the startingShip originated from</returns>
        public static Ship FindMasterShip(Ship startingShip)
        {
            if(startingShip.DockedToShipGUID == 0)
            {
                return startingShip;
            }

            throw new NotImplementedException();
        }
    }
}
