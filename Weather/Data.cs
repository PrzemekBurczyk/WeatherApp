using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    public class Data
    {
        public CurrentCondition[] current_condition { get; set; }
        public Request[] request { get; set; }
        public Weather[] weather { get; set; }
    }
}
