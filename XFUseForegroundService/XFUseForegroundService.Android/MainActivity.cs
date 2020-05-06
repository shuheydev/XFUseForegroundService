using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using XFUseForegroundService.Droid.Services;
using Xamarin.Forms;
using Android.Support.V4.App;
using XFUseForegroundService.Services;
using Shiny;
using XFUseForegroundService.Messages;

namespace XFUseForegroundService.Droid
{
    [Activity(Label = "XFUseForegroundService", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            CreateNotificationFromIntent(Intent);

            this.ShinyOnCreate();

            WireUpForegroundServiceTask();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            AndroidShinyHost.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            CreateNotificationFromIntent(intent);

            this.ShinyOnNewIntent(intent);
        }

        #region Notification
        private void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.Extras.GetString(AndroidNotificationManager.TitleKey);
                string message = intent.Extras.GetString(AndroidNotificationManager.MessageKey);
                DependencyService.Get<INotificationManager>().ReceiveNotification(title, message);
            }
        }
        #endregion

        #region ForegroundService
        private void WireUpForegroundServiceTask()
        {
            MessagingCenter.Subscribe<StartForegroundServiceMessage>(this, nameof(StartForegroundServiceMessage), message =>
            {
                var intent = new Intent(this, typeof(AndroidForegroundService));
                StartService(intent);
            });

            MessagingCenter.Subscribe<StopForegroundServiceMessage>(this, nameof(StopForegroundServiceMessage), message =>
            {
                var intent = new Intent(this, typeof(AndroidForegroundService));
                StopService(intent);
            });
        }
        #endregion

    }
}