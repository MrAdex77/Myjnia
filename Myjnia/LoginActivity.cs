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
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
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
            TextView forgetpassword = FindViewById<TextView>(Resource.Id.ForgetPassword);
            loginButton.Click += LoginButton_Click;
            clicktoregister.Click += Clicktoregister_Click;
            forgetpassword.Click += Forgetpassword_Click;
        }

        private void Forgetpassword_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ForgetPasswordActivity));
            StartActivity(intent);
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

        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        private bool Walidacja()
        {
            if (!String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Password.Text) && isValidEmail(Email.Text))
            {
                return true;
            }
            else
            {
                Toast.MakeText(this, "Email lub Haslo jest nieprawidlowe!", ToastLength.Short).Show();
                return false;
            }
        }

        private async Task Zaloguj()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    Toast.MakeText(this, "Brak internetu!", ToastLength.Short).Show();
                    return;
                }

                using var client = new HttpClient();

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
                var url = "http://80.211.242.184/auth/login";
                var response = await client.PostAsync(url, Content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        Toast.MakeText(this, "Wprowadziles zle dane!", ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.Unauthorized:
                        Toast.MakeText(this, "Nie uwierzytelniono!", ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.NotFound:
                        Toast.MakeText(this, "Nie znaleziono strony!", ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.InternalServerError:
                        string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                        Toast.MakeText(this, blad, ToastLength.Short).Show();
                        break;

                    case HttpStatusCode.OK:
                        Toast.MakeText(this, "Zalogowano!", ToastLength.Short).Show();
                        //handling answer
                        string result = await response.Content.ReadAsStringAsync();
                        var resultObject = JObject.Parse(result);
                        string token = resultObject["token"].ToString();
                        string balans = resultObject["balance"].ToString();
                        try
                        {
                            await SecureStorage.SetAsync("oauth_token", token);
                            await SecureStorage.SetAsync("email", Email.Text);
                            await SecureStorage.SetAsync("balans", balans);
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short).Show();
                            Log.Info("blad", ex.ToString());
                        }
                        Intent intent = new Intent(this, typeof(HomeActivity));
                        intent.PutExtra("email", user.email);
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