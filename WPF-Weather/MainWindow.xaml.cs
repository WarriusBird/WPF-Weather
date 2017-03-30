using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
//---------------------------------------------------------------------------POPULATE THESE VALUES---------------------------------------------------------------------------
        string apiKey = null;//Generate at https://www.wunderground.com/weather/api
        string city = null;
        string state = null; //Two character abbreviation eg - Alabama == AL
        string email = null; //WU developer account email address
        string password = null; //WU developer account password
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            if (apiKey != null && city != null && state != null && email != null && password != null)
            {
                try
                {
                    RenderData(GetData(apiKey, state, city, email, password));
                    StartTimer();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message != null)
                    {
                        Debug.WriteLine(ex.InnerException.Message);
                    }
                    else
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Error:\tPlease populate the required fields within MainWindow.xaml.cs");
                Application.Current.Shutdown();
            }
            
        }

        private void StartTimer()
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 3, 0);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                RenderData(GetData(apiKey, state, city, email, password));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static Rootobject GetData(string key, string state, string town, string email, string password)
        {
            string url = String.Format(@"http://api.wunderground.com/api/{0}/conditions/q/{1}/{2}.json", key, state, town);
            string queryResponse;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = WebRequest.DefaultWebProxy;
            request.Credentials = new NetworkCredential(email, password);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                queryResponse = reader.ReadToEnd();
            }

            Rootobject goddamn = JsonConvert.DeserializeObject<Rootobject>(queryResponse);


            return goddamn;
        }

        public void RenderData(Rootobject weather)
        {
            conditionsLabel.Content = weather.current_observation.weather;
            tempLabel.Content = weather.current_observation.temp_f + "°F";
            feelsLikeLabel.Content = weather.current_observation.feelslike_f + "°F";
            windLabel.Content = weather.current_observation.wind_string;
            windSpeedLabel.Content = weather.current_observation.wind_mph + " mph";
            windGustLabel.Content = weather.current_observation.wind_gust_mph + " mph";
            rainAmountLabel.Content = weather.current_observation.precip_today_in + " in";
            updateTimeLabel.Content = DateTime.Now.ToShortTimeString();
        }

        #region HellaClasses

        public class Rootobject
        {
            public Response response { get; set; }
            public Current_Observation current_observation { get; set; }
        }

        public class Response
        {
            public string version { get; set; }
            public string termsofService { get; set; }
            public Features features { get; set; }
        }

        public class Features
        {
            public int conditions { get; set; }
        }

        public class Current_Observation
        {
            public Image image { get; set; }
            public Display_Location display_location { get; set; }
            public Observation_Location observation_location { get; set; }
            public Estimated estimated { get; set; }
            public string station_id { get; set; }
            public string observation_time { get; set; }
            public string observation_time_rfc822 { get; set; }
            public string observation_epoch { get; set; }
            public string local_time_rfc822 { get; set; }
            public string local_epoch { get; set; }
            public string local_tz_short { get; set; }
            public string local_tz_long { get; set; }
            public string local_tz_offset { get; set; }
            public string weather { get; set; }
            public string temperature_string { get; set; }
            public float temp_f { get; set; }
            public float temp_c { get; set; }
            public string relative_humidity { get; set; }
            public string wind_string { get; set; }
            public string wind_dir { get; set; }
            public int wind_degrees { get; set; }
            public float wind_mph { get; set; }
            public string wind_gust_mph { get; set; }
            public float wind_kph { get; set; }
            public string wind_gust_kph { get; set; }
            public string pressure_mb { get; set; }
            public string pressure_in { get; set; }
            public string pressure_trend { get; set; }
            public string dewpoint_string { get; set; }
            public int dewpoint_f { get; set; }
            public int dewpoint_c { get; set; }
            public string heat_index_string { get; set; }
            public string heat_index_f { get; set; }
            public string heat_index_c { get; set; }
            public string windchill_string { get; set; }
            public string windchill_f { get; set; }
            public string windchill_c { get; set; }
            public string feelslike_string { get; set; }
            public string feelslike_f { get; set; }
            public string feelslike_c { get; set; }
            public string visibility_mi { get; set; }
            public string visibility_km { get; set; }
            public string solarradiation { get; set; }
            public string UV { get; set; }
            public string precip_1hr_string { get; set; }
            public string precip_1hr_in { get; set; }
            public string precip_1hr_metric { get; set; }
            public string precip_today_string { get; set; }
            public string precip_today_in { get; set; }
            public string precip_today_metric { get; set; }
            public string icon { get; set; }
            public string icon_url { get; set; }
            public string forecast_url { get; set; }
            public string history_url { get; set; }
            public string ob_url { get; set; }
            public string nowcast { get; set; }
        }

        public class Image
        {
            public string url { get; set; }
            public string title { get; set; }
            public string link { get; set; }
        }

        public class Display_Location
        {
            public string full { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string state_name { get; set; }
            public string country { get; set; }
            public string country_iso3166 { get; set; }
            public string zip { get; set; }
            public string magic { get; set; }
            public string wmo { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string elevation { get; set; }
        }

        public class Observation_Location
        {
            public string full { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string country_iso3166 { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string elevation { get; set; }
        }

        public class Estimated
        {
        }

        #endregion
    }
}
