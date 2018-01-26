using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionData
{
    /// <summary>
    /// Worker class for retrieving player object from raw json
    /// </summary>
    public static class Players
    {
        /// <summary>
        /// Gets a player given their unique GUID
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public static Player GetPlayer(Int64 GUID)
        {
            if (SaveFile.IsLoaded)
            {
                JObject player = SaveFile.saveData["Players"].Children<JObject>().Where(o => o["GUID"].Value<Int64>() == GUID).FirstOrDefault();
                if (player != null)
                {
                    return new Player(player);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve player GUID {0}", GUID);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Gets a player given their name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>Returns the first player with a given name</returns>
        public static Player GetPlayer(String Name)
        {
            if (SaveFile.IsLoaded)
            {
                JObject player = SaveFile.saveData["Players"].Children<JObject>().Where(o => o["Name"].Value<String>() == Name).FirstOrDefault();
                if (player != null)
                {
                    return new Player(player);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                string message = String.Format("Save file not loaded. Cannot retrieve player name {0}", Name);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Get a full listing of all players
        /// </summary>
        /// <returns></returns>
        public static List<Player> GetPlayers()
        {
            if (!SaveFile.IsLoaded)
            {
                string message = String.Format("Save file not loaded. Cannot retrieve players");
                throw new Exception(message);
            }

            List<Player> players = new List<Player>();
            var playersJson = SaveFile.saveData["Players"].Children<JObject>();
            foreach (var playerJson in playersJson)
            {
                players.Add(new Player(playerJson));
            }

            return players;
        }
    }
}
