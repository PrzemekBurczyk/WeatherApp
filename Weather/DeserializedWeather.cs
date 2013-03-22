using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;

namespace Weather
{
    public class DeserializedWeather
    {
        private static DeserializedWeather instance = null;
        private static WeatherObject deserializedWeather;

        private DeserializedWeather(String json)
        {  
            deserializedWeather = JsonConvert.DeserializeObject<WeatherObject>(json);
        }

        public static DeserializedWeather createNewInstance(String json)
        {
            instance = new DeserializedWeather(json);
            return instance;
        }

        public static DeserializedWeather getInstance()
        {
            return instance;
        }

        public static WeatherObject getWeatherObject()
        {
            return deserializedWeather;
        }

    }
}
