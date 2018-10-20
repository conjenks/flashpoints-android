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

namespace FlashPointsAndroid
{
    public static class Configuration
    {
        public const string ClientId = "1013410030973-i49a6rdthd184v5o84hjffmvvb7ittni.apps.googleusercontent.com";
        public const string Scope = "email";
        public const string RedirectUrl = "com.googleusercontent.apps.123:/oauth2redirect";
    }
}