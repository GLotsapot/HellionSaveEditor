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
		#region Fields

        private JObject player;

		#endregion

		#region Constructors

        internal Player(JObject playerData)
        {
            this.player = playerData;
        }

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the GUID of the character
		/// </summary>
		/// <value>The unique GUID assigned by the server</value>
        public Int64 GUID
        {
            get
            {
                return player["GUID"].Value<Int64>();
            }

			internal set
            {
                player["GUID"] = value;
            }
        }

		/// <summary>
		/// Gets or sets the name of the character
		/// </summary>
		/// <value>The characters name.</value>
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

		/// <summary>
		/// Gets or sets the steam identifier linked to the character
		/// </summary>
		/// <value>The Steam64 ID</value>
        public Int64 SteamId
        {
            get
            {
                return player["SteamId"].Value<Int64>();
            }

            set
            {
                player["SteamId"] = value;
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether this character is alive.
		/// </summary>
		/// <value><c>true</c> if this character is alive; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets or sets the characters health points.
		/// </summary>
		/// <value>The health points.</value>
		public Int32 HealthPoints
		{
			get
			{
				return player["HealthPoints"].Value<Int32>();
			}

			set
			{
				player["HealthPoints"] = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Fills the health of the player
		/// </summary>
        public void FillHealth()
        {
			HealthPoints = player["MaxHealthPoints"].Value<Int32>();
        }

		/// <summary>
		/// Gives the character a complete Mk9 suit if they aren't wearing one already
		/// </summary>
        public void GiveSuit()
        {
            var errorMessage = "I have no idea how to generate a unique suit yet. I'm working on it though.";
            throw new NotImplementedException(errorMessage);
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} - {2}", GUID, Name, SteamId);
        }

		#endregion
    }
}
