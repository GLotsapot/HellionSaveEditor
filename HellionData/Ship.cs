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
        internal Ship(JObject shipData)
        {
        }

        public UInt64 GUID { get; private set; }

        public String Registration { get; set; }

        public String Name { get; set; }

        public UInt64 DockedToShipGUID { get; set; }

        public override string ToString()
        {
            return String.Format("{0} [{1} - {2}]", GUID, Name, Registration);
        }
    }
}
