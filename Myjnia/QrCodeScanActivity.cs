using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Media;
using Xamarin.Essentials;
using ZXing.Mobile;

namespace Myjnia
{
    [Activity(Label = "QrCodeScanActivity")]
    public class QrCodeScanActivity : Activity
    {
        private readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

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
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            var result = await scanner.Scan();

            if (result != null)
            {
                var msg = result.Text;
                var toast = Toast.MakeText(this, msg, ToastLength.Short);
                toast.Show();

                try
                {
                    var oauthToken = await SecureStorage.GetAsync("oauth_token");
                    Send(oauthToken, msg);
                }
                catch (Exception ex)
                {
                    toast = Toast.MakeText(this, "Twoj telefon nie obsluguje SecureStorage!", ToastLength.Short);
                    toast.Show();
                    Log.Info("blad", ex.ToString());
                }
            }
        }

        private async void Send(string Token, string QRCODE)
        {
            using var client = new HttpClient();
            User user = new User
            {
                token = Token,
                qrCode = QRCODE
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            string jsonData = JsonConvert.SerializeObject(user, settings);
            StringContent Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var url = "http://192.168.43.2:5000/machine/compareQrCode";
            var response = await client.PostAsync(url, Content);
            if (response.IsSuccessStatusCode)
            {
                var toast = Toast.MakeText(this, "Wysłano qrCode!", ToastLength.Short);
                toast.Show();
                Finish();
            }
            else
            {
                string blad = "Brak połączenia z serwerem! kod: " + response.StatusCode;
                var toast = Toast.MakeText(this, blad, ToastLength.Short);
                toast.Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}