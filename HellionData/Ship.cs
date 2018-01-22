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

        #region Constructors

        internal Ship(JObject shipData)
        {
			this.ship = shipData;
        }

        #endregion

        #region Properties

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

        public UInt64 DockedToShipGUID
        {
            get
            {
                if(ship["DockedToShipGUID"] != null)
                {
                    return ship["DockedToShipGUID"].Value<UInt64>();
                }
                else
                {
                    return 0;
                }
                
            }
            set
            {
                ship["DockedToShipGUID"] = value;
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
			return String.Format("[{0}] {1} - {2}", GUID, Registration, Name);
        }

        /// <summary>
        /// Fills each resource tank in the ship to it's max capacity
        /// </summary>
        public void ResourceTanksFill()
		{
            foreach (var resourceTank in ship["ResourceTanks"].Children<JObject>())
            {
                var capacity = resourceTank["CargoCompartments"][0]["Capacity"];
                resourceTank["CargoCompartments"][0]["Resources"][0]["Quantity"] = capacity;
            }
        }

        /// <summary>
        /// Sets the health for all parts in the ship to 100
        /// </summary>
		public void DynamicObjectsFix()
		{
            var partObjects = ship["DynamicObjects"].Children<JObject>().Where(o => o["PartType"] != null);

            foreach (var po in partObjects)
            {
                po["Health"] = 100;
            }
        }

        /// <summary>
        /// Sets the tier for all parts in the ship to 4
        /// </summary>
		public void DynamicObjectsUpgrade()
		{
            var partObjects = ship["DynamicObjects"].Children<JObject>().Where(o => o["PartType"] != null);
            foreach (var po in partObjects)
            {
                //HACK: I randomly just picked tier 4. I have no idea if this is even the max level, or even if this may cause problems.
                po["Tier"] = 4;
            }
        }

        /// <summary>
        /// Sets the pressure and quality of each room to 1.0
        /// </summary>
        public void RoomsAirFill()
        {
            foreach (var room in ship["Rooms"].Children<JObject>())
            {
                room["AirPressure"] = 1.0;
                room["AirQuality"] = 1.0;
            }
        }

        /// <summary>
        /// Remove components from a ship if it's health is below a percentage
        /// </summary>
        /// <param name="removalPercentage">The decimal percentage barrier to remove</param>
        public void RemoveBadComponents(uint removalPercentage)
		{
            if (removalPercentage > 100)
            {
                string message = String.Format("Removal percentage must be below 100% you psychopath. {0}% is just retarded.", removalPercentage);
                throw new ArgumentException(message);
            }

            var partObjects = ship["DynamicObjects"].Children<JObject>().Where(o => o["PartType"] != null);

            // NOTE: We have to do this in reverse, as doing it foward and removing causes an Exception
            foreach (var po in partObjects.Reverse())
            {
                if(po["Health"].Value<uint>() < removalPercentage) { po.Remove(); }
            }
        }

		public void DoorsUnlock()
		{
			throw new NotImplementedException ();
		}

        /// <summary>
        /// Goes through each Repair Point and changes it's Health to match MaxHealth
        /// </summary>
        public void RepairPointsFix()
		{
            var rpObjects = ship["RepairPoints"].Children<JObject>();

            int totalHealth = 0;
            foreach (var rp in rpObjects)
            {
                rp["Health"] = rp["MaxHealth"];
                totalHealth = totalHealth + rp["MaxHealth"].ToObject<int>();
            }
            ship["Health"] = totalHealth;
        }

        /// <summary>
        /// Return a ship that this ship is docked to
        /// </summary>
        /// <returns>The parent Ship object, or null if no parent is found</returns>
        public Ship GetParentShip()
        {
            //TODO: Move this to Ships class where it belongs
            if(this.DockedToShipGUID!= 0)
            {
                return Ships.GetShip(this.DockedToShipGUID);
            }
            else
            {
                return null;
            }            
        }

        #endregion
    }
}
