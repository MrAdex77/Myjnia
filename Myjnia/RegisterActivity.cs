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
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Myjnia
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : AppCompatActivity
    {
        private EditText Email;
        private EditText Password;
        private EditText PasswordConfirm;

        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            // Create your application here

            //variables
            Email = FindViewById<EditText>(Resource.Id.emailRegisterText);
            Password = FindViewById<EditText>(Resource.Id.passwordRegisterText);
            PasswordConfirm = FindViewById<EditText>(Resource.Id.passwordconfirmRegisterText);

            Button registerButton = FindViewById<Button>(Resource.Id.RegisterButton);
            registerButton.Click += RegisterButton_Click;
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            if (Walidacja())
            {
                await Register();
            }
        }

        private bool Walidacja()
        {
            if (!String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(PasswordConfirm.Text))
            {
                return true;
            }
            else
            {
                Toast.MakeText(this, "Email lub Haslo jest puste!", ToastLength.Short).Show();
                return false;
            }
        }

        private async Task Register()
        {
            try
            {
                using var client = new HttpClient();
                //get request
                User user = new User
                {
                    email = Email.Text,
                    password = Password.Text
                };
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                string jsonData = JsonConvert.SerializeObject(user, settings);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //lokalne
                var url = "http://80.211.242.184/auth/register";
                var response = await client.PostAsync(url, Content);
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
                        Toast.MakeText(this, "Zarejestrowano!", ToastLength.Short).Show();
                        Intent intent = new Intent(this, typeof(LoginActivity));
                        StartActivity(intent);
                        break;
                }
            }
            catch (IOException e)
            {
                Toast.MakeText(this, e.ToString(), ToastLength.Long).Show();
                Log.Info("blad", e.ToString());
            }
        }
    }
}