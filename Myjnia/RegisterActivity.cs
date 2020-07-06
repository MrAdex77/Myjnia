using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

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
        private string email;
        private int password;
        private int password2;

        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            // Create your application here

            //variables
            Button registerButton = FindViewById<Button>(Resource.Id.RegisterButton);
            registerButton.Click += RegisterButton_Click;
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            Register();
        }

        private async void Register()
        {
            try
            {
                //email = FindViewById<TextInputLayout>(Resource.Id.emailRegisterText).EditText.Text;
                //password = Convert.ToInt32(FindViewById<TextInputLayout>(Resource.Id.passwordRegisterText).EditText.Text);
                //password2 = Convert.ToInt32(FindViewById<TextInputLayout>(Resource.Id.passwordconfirmRegisterText).EditText.Text);
                using var client = new HttpClient();
                //get request
                User user = new User();
                user.email = "adrian123123123@gmail.com";
                user.password = "Adrian123@";
                string jsonData = JsonConvert.SerializeObject(user);
                StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //lokalne
                var url = "http://192.168.43.2:5000/auth/register";
                //publiczne
                // var url = "http://80.211.242.184/auth/register";
                var result = await client.PostAsync(url, Content);

                //handling answer
                //var wiadomosc = JsonConvert.DeserializeObject(result);
                var re = result.StatusCode.ToString();
                var toast = Toast.MakeText(this, re, ToastLength.Short);
                toast.Show();
            }
            catch (IOException e)
            {
                Log.Info("myjnia", e.ToString());
            }
        }
    }
}