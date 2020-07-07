using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
            //Click event handlers
            qrcodeLayout.Click += QrcodeLayout_Click;

            //
            welcomeMessage.Text = "Witaj, " + Intent.GetStringExtra("email") + "!";
        }

        private void QrcodeLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(QrCodeScanActivity));
            //intent.PutExtra("topic", "Business");
            StartActivity(intent);
        }
    }
}