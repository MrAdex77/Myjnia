using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using ZXing.Mobile;

namespace Myjnia
{
    [Activity(Label = "QrCodeScanActivity")]
    public class QrCodeScanActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)

        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

            // Create your application here
            //await RequestPermissions(permissionGroup, 0);
            //var status = await Permissions.RequestAsync(permissionGroup);
            ScanCode();
        }

        private async void ScanCode()
        {
            MobileBarcodeScanner.Initialize(Application);
            var scanner = new MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                var msg = result.Text;
                Toast.MakeText(this, msg, ToastLength.Short).Show();
                Finish();
                Intent intent = new Intent(this, typeof(MyjniaOpcjeActivity));
                intent.PutExtra("qrcode", msg);
                StartActivity(intent);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}