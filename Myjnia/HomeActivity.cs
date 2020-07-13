using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    [Activity(Label = "HomeActivity")]
    public class HomeActivity : Activity
    {
        //layouts
        private LinearLayout qrcodeLayout;

        private LinearLayout StanMaszynLayout;
        private LinearLayout KontoLayout;
        private LinearLayout UstawieniaLayout;
        private TextView welcomeMessage;
        private TextView accountBalance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Home);
            // Create your application here

            //View Setups
            qrcodeLayout = (LinearLayout)FindViewById(Resource.Id.qrcodeLayout);
            StanMaszynLayout = (LinearLayout)FindViewById(Resource.Id.StanMaszynLayout);
            KontoLayout = (LinearLayout)FindViewById(Resource.Id.KontoLayout);
            UstawieniaLayout = (LinearLayout)FindViewById(Resource.Id.UstawieniaLayout);
            welcomeMessage = (TextView)FindViewById(Resource.Id.WelcomeMessage);
            accountBalance = (TextView)FindViewById(Resource.Id.AccountBalance);
            //Click event handlers
            qrcodeLayout.Click += QrcodeLayout_Click;
            StanMaszynLayout.Click += StanMaszynLayout_Click;
            KontoLayout.Click += KontoLayout_Click;
            UstawieniaLayout.Click += UstawieniaLayout_Click;

            //
            Profil();
        }

        private void UstawieniaLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }

        private void KontoLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ProfileActivity));
            StartActivity(intent);
        }

        private void StanMaszynLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(StanMaszynActivity));
            StartActivity(intent);
        }

        private async void Profil()
        {
            try
            {
                var balans = await SecureStorage.GetAsync("balans");
                welcomeMessage.Text = "Witaj, " + Intent.GetStringExtra("email") + "!";
                accountBalance.Text = "Saldo: " + balans + " zł";
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short).Show();
                Log.Info("blad", ex.ToString());
            }
        }

        private void QrcodeLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(QrCodeScanActivity));
            //intent.PutExtra("topic", "Business");
            StartActivity(intent);
        }
    }
}