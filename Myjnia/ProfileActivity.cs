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
    [Activity(Label = "profileActivity")]
    public class ProfileActivity : Activity
    {
        private TextView WelcomeMessage;
        private TextView AccountBalance;
        private Button ChangePassword;
        private Button AddMoney;
        private Button Logout;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.profile);
            // Create your application here

            WelcomeMessage = (TextView)FindViewById(Resource.Id.ProfileWelcomeMessage);
            AccountBalance = (TextView)FindViewById(Resource.Id.AccountProfileBalance);
            ChangePassword = FindViewById<Button>(Resource.Id.ProfileChangePassword);
            AddMoney = FindViewById<Button>(Resource.Id.ProfileAddMoney);
            Logout = FindViewById<Button>(Resource.Id.ProfileLogout);

            Logout.Click += Logout_Click;

            //
            await Profil();
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            Wyloguj();
        }

        private async Task Profil()
        {
            try
            {
                await Send();
                var email = await SecureStorage.GetAsync("email");
                var balans = await SecureStorage.GetAsync("balans");
                WelcomeMessage.Text = "Witaj, " + email + "!";
                AccountBalance.Text = "Saldo: " + balans + " zł";
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short).Show();
                Log.Info("blad", ex.ToString());
            }
        }

        private async Task Send()
        {
            try
            {
                var url = "http://80.211.242.184/user/profile";
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

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var resultObject = JObject.Parse(result);
                    string email = resultObject["email"].ToString();
                    string balans = resultObject["balance"].ToString();
                    try
                    {
                        await SecureStorage.SetAsync("email", email);
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

        private void Wyloguj()
        {
            try
            {
                SecureStorage.RemoveAll();
                Toast.MakeText(this, "Wylogowano!", ToastLength.Short).Show();
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short).Show();
                Log.Info("blad", ex.ToString());
            }
        }
    }
}