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
using Shiny;

namespace XFUseForegroundService.Droid
{
    [Application]
    public class ShinyApplication:Application
    {
        public ShinyApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            AndroidShinyHost.Init(this, new AppShinyStartup());
        }
    }
}