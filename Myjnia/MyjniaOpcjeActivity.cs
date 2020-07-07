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
using Xamarin.Essentials;

namespace Myjnia
{
    [Activity(Label = "MyjniaOpcjeActivity")]
    public class MyjniaOpcjeActivity : Activity
    {
        private Button btSzybkie;
        private Button btEkspert;
        private Button btPremium;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MyjniaOpcje);
            // Create your application here
            btSzybkie = FindViewById<Button>(Resource.Id.MyjniaOpcjaSzybkie);
            btEkspert = FindViewById<Button>(Resource.Id.MyjniaOpcjaEkspert);
            btPremium = FindViewById<Button>(Resource.Id.MyjniaOpcjaPremium);
            btSzybkie.Click += BtSzybkie_Click;
            btEkspert.Click += BtEkspert_Click;
            btPremium.Click += BtPremium_Click;
        }

        private async void BtPremium_Click(object sender, EventArgs e)
        {
            await Send("Premium");
        }

        private async void BtEkspert_Click(object sender, EventArgs e)
        {
            await Send("Ekspert");
        }

        private async void BtSzybkie_Click(object sender, EventArgs e)
        {
            await Send("Szybkie");
        }

        private async Task Send(string opcja)
        {
            string oauthToken = string.Empty;
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
                option = opcja
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            string jsonData = JsonConvert.SerializeObject(user, settings);
            StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var url = "http://192.168.43.2:5000/machine/optionsMachine";
            var response = await client.PostAsync(url, Content);
            if (response.IsSuccessStatusCode)
            {
                Toast.MakeText(this, "Czas sie odlicza myj auto!", ToastLength.Short).Show();
            }
            else
            {
                string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                var toast = Toast.MakeText(this, blad, ToastLength.Short);
                toast.Show();
            }
        }
    }
}