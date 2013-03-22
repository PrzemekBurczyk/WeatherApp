using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    class ForecastItem
    {
        public String date { get; set; }
        public String imgUrl { get; set; }
        public String imgDesc { get; set; }
        public String temperature { get; set; }
        public String wind { get; set; }
    }
}
