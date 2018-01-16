using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionData
{
    public class Player
    {
        private JObject player;

        internal Player(JObject playerData)
        {
            this.player = playerData;
        }

        public UInt64 GUID
        {
            get
            {
                return player["GUID"].Value<UInt64>();
            }

            private set
            {
                player["GUID"] = value;
            }
        }

        public String Name
        {
            get
            {
                return player["Name"].Value<String>();
            }

            set
            {
                player["Name"] = value;
            }
        }

        public UInt64 SteamId
        {
            get
            {
                return player["SteamId"].Value<UInt64>();
            }

            set
            {
                player["SteamId"] = value;
            }
        }

        public Boolean IsAlive
        {
            get
            {
                return player["IsAlive"].Value<Boolean>();
            }

            set
            {
                player["IsAlive"] = value;
            }
        }

        public void FillHealth()
        {
            player["HealthPoints"] = player["MaxHealthPoints"];
        }

        public void GiveSuit()
        {
            var errorMessage = "I have no idea how to generate a unique suit yet. I'm working on it though.";
            throw new NotImplementedException(errorMessage);
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} - {2}", GUID, Name, SteamId);
        }
    }
}
