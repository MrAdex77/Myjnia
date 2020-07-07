using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private EditText Email;
        private EditText Password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            // Create your application here

            //variables
            Email = FindViewById<EditText>(Resource.Id.emailLoginText);
            Password = FindViewById<EditText>(Resource.Id.passwordLoginText);
            Button loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            TextView clicktoregister = FindViewById<TextView>(Resource.Id.clickToRegister);
            loginButton.Click += LoginButton_Click;
            clicktoregister.Click += Clicktoregister_Click;
        }

        private void Clicktoregister_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (Walidacja())
            {
                await Zaloguj();
            }
        }

        private bool Walidacja()
        {
            if (!String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }
            else
            {
                Toast.MakeText(this, "Email lub Haslo jest puste!", ToastLength.Short).Show();
                return false;
            }
        }

        private async Task Zaloguj()
        {
            try
            {
                using var client = new HttpClient();

                //var result = await client.GetAsync("http://192.168.43.2:5000/auth/register");
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                User user = new User
                {
                    email = Email.Text,
                    password = Password.Text
                };
                string jsonData = JsonConvert.SerializeObject(user, settings);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var url = "http://192.168.43.2:5000/auth/login";
                var response = await client.PostAsync(url, Content);

                //handling answer
                string result = await response.Content.ReadAsStringAsync();
                //var re = response.StatusCode.ToString();
                // testowekonto@gmail.com Adex123@
                //Toast.MakeText(this, re, ToastLength.Short).Show();

                //obsługa tokenu
                //Toast.MakeText(this, re, ToastLength.Short).Show();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        Toast.MakeText(this, "Wprowadziles zle dane!", ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.Unauthorized:
                        Toast.MakeText(this, "Nie uwierzytelniono!", ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.InternalServerError:
                        string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                        Toast.MakeText(this, blad, ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.OK:
                        //token
                        var resultObject = JObject.Parse(result);
                        string token = resultObject["token"].ToString();
                        try
                        {
                            await SecureStorage.SetAsync("oauth_token", token);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short).Show();
                            Log.Info("blad", ex.ToString());
                        }
                        Toast.MakeText(this, "Zalogowano!", ToastLength.Short).Show();
                        Intent intent = new Intent(this, typeof(HomeActivity));
                        StartActivity(intent);
                        break;
                }
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
                Log.Info("blad", e.ToString());
            }
        }
    }
}