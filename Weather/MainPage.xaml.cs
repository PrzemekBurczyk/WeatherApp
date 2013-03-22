using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Newtonsoft.Json;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace Weather
{
    public partial class MainPage : PhoneApplicationPage
    {
        private double currentDegree;
        private IsolatedStorageSettings userSettings;
        private List<ForecastItem> forecastList;

        public MainPage()
        {
            InitializeComponent();
            userSettings = IsolatedStorageSettings.ApplicationSettings;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            currentDegree = 0;

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection is not available!");
            }
            else
            {
                loadWeatherData();
            }

            if (isSettingsSet())
            {
                loadSavedSettings();
            }
            else
            {
                setSettingsDefault();
            }
        }

        public bool isSettingsSet()
        {
            if (!userSettings.Contains("temperature") || !userSettings.Contains("wind"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void loadSavedSettings()
        {
            if ((String)userSettings["temperature"] == "F")
            {
                temperatureSwitch.IsChecked = true;
                temperatureSwitch.Content = "F";
            }
            else if ((String)userSettings["temperature"] == "C")
            {
                temperatureSwitch.IsChecked = false;
                temperatureSwitch.Content = "C";
            }
            if ((String)userSettings["wind"] == "mph")
            {
                windSwitch.IsChecked = true;
                windSwitch.Content = "mph";
            }
            else if ((String)userSettings["wind"] == "kph")
            {
                windSwitch.IsChecked = false;
                windSwitch.Content = "kph";
            }
            
        }

        public void setSettingsDefault()
        {
            if (temperatureSwitch.IsChecked == true)
            {
                temperatureSwitch.Content = "F";
                userSettings.Add("temperature", "F");
            }
            else
            {
                temperatureSwitch.Content = "C";
                userSettings.Add("temperature", "C");
            }
            if (windSwitch.IsChecked == true)
            {
                windSwitch.Content = "mph";
                userSettings.Add("wind", "mph");
            }
            else
            {
                windSwitch.Content = "kph";
                userSettings.Add("wind", "kph");
            }
        }

        public void loadWeatherData()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("http://free.worldweatheronline.com/feed/weather.ashx?q=Cracow,Poland&format=json&num_of_days=5&key=652e93c9d6153629131202"));
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            DeserializedWeather.createNewInstance(e.Result);
            myPanorama.Title = "Weather: " + DeserializedWeather.getWeatherObject().data.request[0].query;
            loadPanoramaItems();
        }

        private void loadPanoramaItems()
        {
            loadCurrentCondition();
            loadForecast();
            loadArrow();
        }

        private void loadCurrentCondition()
        {
            WeatherObject weatherObject = DeserializedWeather.getWeatherObject();
            BitmapImage icon = new BitmapImage(new Uri(weatherObject.data.current_condition[0].weatherIconUrl[0].value));
            currentWeatherIcon.Source = icon;
            currentWeatherDescription.Text = weatherObject.data.current_condition[0].weatherDesc[0].value;
            observationTime.Text = "Observation time: " + weatherObject.data.current_condition[0].observation_time;
            cloudcover.Text = "Cloudcover: " + weatherObject.data.current_condition[0].cloudcover + "%";
            humidity.Text = "Humidity: " + weatherObject.data.current_condition[0].humidity + "%";
            pressure.Text = "Pressure: " + weatherObject.data.current_condition[0].pressure + " hPa";
            visibility.Text = "Visibility: " + weatherObject.data.current_condition[0].visibility + "%";
            windDirection.Text = "Wind direction: " + weatherObject.data.current_condition[0].winddir16Point;
            if ((String)userSettings["temperature"] == "C")
            {
                temperature.Text = "Temperature: " + weatherObject.data.current_condition[0].temp_C + " °C";
            }
            else if ((String)userSettings["temperature"] == "F")
            {
                temperature.Text = "Temperature: " + weatherObject.data.current_condition[0].temp_F + " F";
            }
            if ((String)userSettings["wind"] == "kph")
            {
                windSpeed.Text = "Wind: " + weatherObject.data.current_condition[0].windspeedKmph + " kph";
            }
            else if ((String)userSettings["wind"] == "mph")
            {
                windSpeed.Text = "Wind: " + weatherObject.data.current_condition[0].windspeedMiles + " mph";
            }
        }

        private void loadArrow()
        {
            Duration duration = new Duration(TimeSpan.FromMilliseconds(4000));
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.Duration = duration;
            ElasticEase be = new ElasticEase();
            be.Oscillations = 1;
            be.Springiness = 4;
            be.EasingMode = EasingMode.EaseOut;
            myDoubleAnimation.EasingFunction = be;
            Storyboard sb = new Storyboard();
            sb.Duration = duration;
            arrow.RenderTransform = new RotateTransform();
            Storyboard.SetTarget(myDoubleAnimation, arrow.RenderTransform);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(RotateTransform.AngleProperty));
            myDoubleAnimation.From = currentDegree;
            double degree = Convert.ToDouble(DeserializedWeather.getWeatherObject().data.current_condition[0].winddirDegree);
            myDoubleAnimation.To = degree;
            currentDegree = degree;
            sb.Children.Add(myDoubleAnimation);
            sb.Begin();
        }

        private void loadForecast()
        {
            String tmpMax = null;
            String tmpMin = null;
            String windSpeed = null;
            forecastList = new List<ForecastItem>();
            WeatherObject weatherObject = DeserializedWeather.getWeatherObject();
            for (int i = 0; i < weatherObject.data.weather.Count(); i++)
            {
                if ((String)userSettings["temperature"] == "C")
                {
                    tmpMax = weatherObject.data.weather[i].tempMaxC + " °C";
                    tmpMin = weatherObject.data.weather[i].tempMinC + " °C";
                }
                else if ((String)userSettings["temperature"] == "F")
                {
                    tmpMax = weatherObject.data.weather[i].tempMaxF + " F";
                    tmpMin = weatherObject.data.weather[i].tempMinF + " F";
                }
                if ((String)userSettings["wind"] == "kph")
                {
                    windSpeed = weatherObject.data.weather[i].windspeedKmph + " kph";
                }
                else if ((String)userSettings["wind"] == "mph")
                {
                    windSpeed = weatherObject.data.weather[i].windspeedMiles + " mph";
                }
                forecastList.Add(new ForecastItem
                {
                    date = weatherObject.data.weather[i].date,
                    imgUrl = weatherObject.data.weather[i].weatherIconUrl[0].value,
                    imgDesc = weatherObject.data.weather[i].weatherDesc[0].value,
                    temperature = "Temperature Min: " + tmpMin + " Max: " + tmpMax,
                    wind = "Wind Direction: " + weatherObject.data.weather[i].winddir16Point + " Speed: " + windSpeed
                });
            }
            forecastListView.ItemsSource = forecastList;
        }

        private void temperatureSwitch_Checked_1(object sender, RoutedEventArgs e)
        {
            temperatureSwitch.Content = "F";
            userSettings["temperature"] = "F";
            if (DeserializedWeather.getInstance() != null && DeserializedWeather.getWeatherObject() != null)
            {
                loadPanoramaItems();
            }
        }

        private void temperatureSwitch_Unchecked_1(object sender, RoutedEventArgs e)
        {
            temperatureSwitch.Content = "C";
            userSettings["temperature"] = "C";
            if (DeserializedWeather.getInstance() != null && DeserializedWeather.getWeatherObject() != null)
            {
                loadPanoramaItems();
            }
        }

        private void windSwitch_Checked_1(object sender, RoutedEventArgs e)
        {
            windSwitch.Content = "mph";
            userSettings["wind"] = "mph";
            if (DeserializedWeather.getInstance() != null && DeserializedWeather.getWeatherObject() != null)
            {
                loadPanoramaItems();
            }
        }

        private void windSwitch_Unchecked_1(object sender, RoutedEventArgs e)
        {
            windSwitch.Content = "kph";
            userSettings["wind"] = "kph";
            if (DeserializedWeather.getInstance() != null && DeserializedWeather.getWeatherObject() != null)
            {
                loadPanoramaItems();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("Network connection is not available!");
            }
            else
            {
                loadWeatherData();
            }
        }
    }
}