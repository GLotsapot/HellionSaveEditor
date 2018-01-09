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
        public static Ship GetShip(UInt64 GUID)
        {
            if (SaveFile.IsLoaded)
            {
                JObject ship = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["GUID"].Value<string>() == GUID.ToString()).FirstOrDefault();
                return new Ship(ship);
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve ship GUID {0}", GUID);
                throw new Exception(message);
            }
        }

		public static Ship GetShip(string NameOrRegistration)
		{
			if (SaveFile.IsLoaded)
			{
				JObject ship = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["Registration"].Value<string>() == NameOrRegistration || o["Name"].Value<string>() == NameOrRegistration).FirstOrDefault();
				return new Ship(ship);
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
                IEnumerable<JObject> shipChildren = SaveFile.saveData["Ships"].Children<JObject>().Where(o => o["DockedToShipGUID"].Value<string>() == parentShip.GUID.ToString());

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
    }
}
