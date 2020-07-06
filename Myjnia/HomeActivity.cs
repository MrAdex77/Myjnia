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

        private LinearLayout spaceLayout;
        private LinearLayout geographyLayout;
        private LinearLayout engineeringLayout;
        private LinearLayout programmingLayout;
        private LinearLayout businessLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Home);
            // Create your application here

            //View Setups
            qrcodeLayout = (LinearLayout)FindViewById(Resource.Id.qrcodeLayout);
            spaceLayout = (LinearLayout)FindViewById(Resource.Id.spaceLayout);
            geographyLayout = (LinearLayout)FindViewById(Resource.Id.geographyLayout);
            engineeringLayout = (LinearLayout)FindViewById(Resource.Id.engineeringLayout);
            programmingLayout = (LinearLayout)FindViewById(Resource.Id.programmingLayout);
            businessLayout = (LinearLayout)FindViewById(Resource.Id.businessLayout);
            //Click event handlers
            qrcodeLayout.Click += QrcodeLayout_Click;

            //
        }

        private void QrcodeLayout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(QrCodeScanActivity));
            //intent.PutExtra("topic", "Business");
            StartActivity(intent);
        }
    }
}