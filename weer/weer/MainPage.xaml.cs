using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace weer
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            ApiHelper.InitializeClient();         
            InitializeComponent();
        }

        public void ZoekPlaatsButtonClick(Object sender, EventArgs args)
        {           
            GetAndPlaceInfo();
        }

        public async void GetAndPlaceInfo() {
            try
            {
                ApiHelper.InitializeClient();
                var plaats = PlaatsInput.Text;
                Info info = await GetWeather(plaats);                             
               
                string timezone = info.timezone;
                string huidigedatumgeg = DateTime.Now.AddHours(2).ToString("dd/MM/yyyy HH:mm");

                if (info.name.Contains(" "))
                {
                    string newword = string.Empty;
                    foreach (var letter in info.name)
                    {
                        if (!letter.Equals(' '))
                        {
                            newword += letter.ToString();
                        }
                        else
                        {
                            TitelPlaats.Text = newword;
                            break;
                        }
                    }
                }
                else
                {
                    TitelPlaats.Text = info.name;
                }

                Temperatuur.Text = Math.Round(info.main.temp).ToString() + "°C";
                MinMaxTemp.Text = Math.Round(info.main.temp_min).ToString() + "°C | " + Math.Round(info.main.temp_max).ToString() + "°C";

                foreach (var item in info.weather)
                {
                    if (item.description.Equals("broken clouds"))
                    {
                        Description.Text = "onderbroken bewolking";
                    }
                    else if (item.description.Equals("drizzle"))
                    {
                        Description.Text = "motregen";
                    }
                    else if (item.description.Equals("light rain"))
                    {
                        Description.Text = "lichte regen";
                    }
                    else
                    {
                        Description.Text = item.description;
                    }
                }

                DatumGegevens.Text = huidigedatumgeg;

                DateTime rise = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                rise = rise.AddSeconds(info.sys.sunrise).AddSeconds(int.Parse(timezone));
                Sunrise.Text = rise.TimeOfDay.ToString();
                DateTime set = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                set = set.AddSeconds(info.sys.sunset).AddSeconds(int.Parse(timezone));
                Sunset.Text = set.TimeOfDay.ToString();

                string[] caridnals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };

                Windsnelheid.Text = "Wind:  " + info.wind.speed.ToString() + "m/s  " + caridnals[(int)Math.Round(((decimal)info.wind.deg % 360) / 45)];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Info> GetWeather(string plaats)
        {
            string url = string.Empty;
            string Apikey = "ff7df3738a3709320fa4c8c259d5a6e2";
            string Unit = "metric";

            if (plaats.Equals("") || plaats == null)
            {
                url = null;
            }
            else if (!int.TryParse(plaats, out int Result))
            {
                url = $"https://api.openweathermap.org/data/2.5/weather?q={plaats}&APPID={Apikey}&units={Unit}";
            }
            else
            {
                url = $"https://api.openweathermap.org/data/2.5/weather?zip={plaats},be&APPID={Apikey}&units={Unit}";
            }

            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string response = await client.GetStringAsync(url);

            Info info = JsonConvert.DeserializeObject<Info>(JObject.Parse(response).ToString());
            return info;
        }
    }
}
