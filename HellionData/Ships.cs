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
            if (SaveFile.IsLoaded)
            {
                // TODO: Get all ships and return them, optionally just ones that aren't docked to anything.
                // TODO: Maybe thread this to improve retrieval?
                throw new NotImplementedException();
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve ships");
                throw new Exception(message);
            }
        }
    }
}
