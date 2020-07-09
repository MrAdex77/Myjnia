using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
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