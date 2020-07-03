using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Myjnia
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            // Create your application here
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

        private void LoginButton_Click(object sender, EventArgs e)
        {
            Pobierz();
        }

        private async void Pobierz()
        {
            try
            {
                using var client = new HttpClient();
                //get request
                //var uri = "http://webcode.me";
                //var result = await client.GetStringAsync(uri);
                var result = await client.GetAsync("http://80.211.242.184/adrian");
                //handling answer
                //var wiadomosc = JsonConvert.DeserializeObject(result);
                var re = result.StatusCode.ToString();
                var toast = Toast.MakeText(this, re, ToastLength.Short);
                toast.Show();
            }
            catch (IOException e)
            {
                Log.Info("app", e.ToString());
            }
        }

        private async void Register()
        {
        }
    }
}