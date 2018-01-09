using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionData
{
    public class Ship
    {

		private JObject ship;

        internal Ship(JObject shipData)
        {
			this.ship = shipData;
        }
			
        public UInt64 GUID 
		{ 
			get 
			{
				return ship["GUID"].Value<UInt64>();
			}

			private set
			{
				ship["GUID"] = value;
			}
		}

        public String Registration 
		{ 
			get {
				return ship ["Registration"].Value<string> ();
			}
			set {
				ship ["Registration"] = value;
			}
		}

        public String Name
		{ 
			get {
				return ship ["Name"].Value<string> ();
			}
			set {
				ship ["Name"] = value;
			}
		}

        public UInt64 DockedToShipGUID { get; set; }

        public override string ToString()
        {
			return String.Format("[{0}] {1} - {2}", GUID, Registration, Name);
        }

		public void ResourceTanksFill()
		{
			throw new NotImplementedException ();
		}

		public void DynamicObjectsFix()
		{
			throw new NotImplementedException ();
		}

		public void DynamicObjectsUpgrade()
		{
			throw new NotImplementedException ();
		}

		public void RoomsAir()
		{
			throw new NotImplementedException ();
		}

		public void RemoveBadComponents(double removalPercentage)
		{
			throw new NotImplementedException ();
		}

		public void DoorsUnlock()
		{
			throw new NotImplementedException ();
		}

		public void RepairPointsFix()
		{
			throw new NotImplementedException ();
		}
    }
}
