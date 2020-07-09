using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;

namespace Myjnia
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, NoHistory = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }
    }
}