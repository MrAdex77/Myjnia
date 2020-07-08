using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace Myjnia
{
    [Activity(Label = "MyjniaOpcjeActivity")]
    public class MyjniaOpcjeActivity : Activity
    {
        private Button btSzybkie;
        private Button btEkspert;
        private Button btPremium;
        private Button btZakoncz;
        private TextView licznik;
        private DateTime dateTime;
        private int timerCounter = 0;
        private System.Timers.Timer countDown = new System.Timers.Timer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyjniaOpcje);
            // Create your application here
            btSzybkie = FindViewById<Button>(Resource.Id.MyjniaOpcjaSzybkie);
            btEkspert = FindViewById<Button>(Resource.Id.MyjniaOpcjaEkspert);
            btPremium = FindViewById<Button>(Resource.Id.MyjniaOpcjaPremium);
            btZakoncz = FindViewById<Button>(Resource.Id.MyjniaOpcjaZakoncz);
            licznik = FindViewById<TextView>(Resource.Id.timeCounterTextView);
            btSzybkie.Click += BtSzybkie_Click;
            btEkspert.Click += BtEkspert_Click;
            btPremium.Click += BtPremium_Click;
            btZakoncz.Click += BtZakoncz_Click;
            //variables

            countDown.Interval = 1000;
            countDown.Elapsed += CountDown_Elapsed;
            countDown.Enabled = false;
        }

        private async void BtZakoncz_Click(object sender, EventArgs e)
        {
            await Send();
            countDown.Enabled = false;
        }

        private void CountDown_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime dt = new DateTime();
            dt = dateTime.AddSeconds(-1);
            var dateDifference = dateTime.Subtract(dt);
            dateTime -= dateDifference;

            timerCounter--;
            //Ended
            RunOnUiThread(async () =>
           {
               licznik.Text = dateTime.ToString("mm:ss");
               if (timerCounter == 0)
               {
                   Toast.MakeText(this, "Czas sie skonczyl!", ToastLength.Short).Show();
                   countDown.Enabled = false;
                   await Send();
               }
           });
        }

        private void StartCzas(double ile)
        {
            dateTime = new DateTime();
            dateTime = dateTime.AddMinutes(ile);
            licznik.Text = dateTime.ToString("mm:ss");

            countDown.Enabled = true;
        }

        private async void BtPremium_Click(object sender, EventArgs e)
        {
            await Send("premium");
        }

        private async void BtEkspert_Click(object sender, EventArgs e)
        {
            await Send("expert");
        }

        private async void BtSzybkie_Click(object sender, EventArgs e)
        {
            await Send("szybkie");
        }

        private async Task Send(string opcja)
        {
            try
            {
                var url = "http://80.211.242.184/machine/startMachine";
                string oauthToken = string.Empty;
                string qrcode = Intent.GetStringExtra("qrcode");
                try
                {
                    oauthToken = await SecureStorage.GetAsync("oauth_token");
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short);
                    Log.Info("blad", ex.ToString());
                }
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                User user = new User
                {
                    option = opcja,
                    qrCode = qrcode
                };
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                string jsonData = JsonConvert.SerializeObject(user, settings);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, Content);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultObject = JObject.Parse(result);
                    string czas = resultObject["time"].ToString();
                    string balans = resultObject["balance"].ToString();
                    timerCounter = Convert.ToInt32(czas) * 60;
                    StartCzas(Convert.ToDouble(czas));
                    try
                    {
                        await SecureStorage.SetAsync("balans", balans);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short);
                        Log.Info("blad", ex.ToString());
                    }
                }
                else
                {
                    string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                    var toast = Toast.MakeText(this, blad, ToastLength.Short);
                    toast.Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short);
                Log.Info("blad", ex.ToString());
            }
        }

        private async Task Send()
        {
            //metoda wysla do serwra zapytanie konczoce i zwolniajce myjnie
            try
            {
                var url = "http://80.211.242.184/machine/stopMachine";
                string oauthToken = string.Empty;
                string qrcode = Intent.GetStringExtra("qrcode");
                try
                {
                    oauthToken = await SecureStorage.GetAsync("oauth_token");
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short);
                    Log.Info("blad", ex.ToString());
                }
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                User user = new User
                {
                    qrCode = qrcode
                };
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                string jsonData = JsonConvert.SerializeObject(user, settings);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, Content);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Toast.MakeText(this, "Zwolniono myjnie!", ToastLength.Short);
                }
                else
                {
                    string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                    var toast = Toast.MakeText(this, blad, ToastLength.Short);
                    toast.Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short);
                Log.Info("blad", ex.ToString());
            }
        }
    }
}