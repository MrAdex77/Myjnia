using System;
using System.Collections.Generic;
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
using Plugin.Connectivity;

namespace Myjnia
{
    [Activity(Label = "ForgetPasswordActivity")]
    public class ForgetPasswordActivity : Activity
    {
        private EditText Email;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ForgetPassword);
            // Create your application here
            //variables
            Email = FindViewById<EditText>(Resource.Id.emailForgetText);
            Button sendButton = FindViewById<Button>(Resource.Id.ForgetPasswordButton);
            sendButton.Click += SendButton_Click;
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            if (Walidacja())
            {
                await Send();
            }
        }

        private bool Walidacja()
        {
            if (!String.IsNullOrEmpty(Email.Text) && isValidEmail(Email.Text))
            {
                return true;
            }
            else
            {
                Toast.MakeText(this, "Email jest nieprawidlowy!", ToastLength.Short).Show();
                return false;
            }
        }

        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        private async Task Send()
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
                    email = Email.Text
                };
                string jsonData = JsonConvert.SerializeObject(user, settings);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var url = "http://80.211.242.184/user/SendRemindPasswordToEmail";
                var response = await client.PostAsync(url, Content);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        Toast.MakeText(this, "Nie znaleziono emaila w bazie!", ToastLength.Short).Show();
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
                        Toast.MakeText(this, "Wysłano przypomnienie, sprawdź pocztę!", ToastLength.Short).Show();
                        //handling answers
                        Intent intent = new Intent(this, typeof(LoginActivity));
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