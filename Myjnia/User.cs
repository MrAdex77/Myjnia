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
    public class User
    {
        public string email { get; set; }
        public string password { get; set; }

        public string token { get; set; }

        public string qrCode { get; set; }

        public string option { get; set; }
    }
}