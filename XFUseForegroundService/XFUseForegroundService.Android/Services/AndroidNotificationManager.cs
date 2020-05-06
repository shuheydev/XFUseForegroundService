using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using XFUseForegroundService.Services;

[assembly: Xamarin.Forms.Dependency(typeof(XFUseForegroundService.Droid.Services.AndroidNotificationManager))]
namespace XFUseForegroundService.Droid.Services
{
    public class AndroidNotificationManager : INotificationManager
    {
        public event EventHandler NotificationReceived;

        NotificationManager _notificationManager;
        private string _channelId;
        private string _channelName;
        private string _channelDescription;
        private int _pendingIntentId;

        private int _messageId = -1;
        private bool _channelInitialized = false;

        public static readonly string TitleKey = "title";
        public static readonly string MessageKey = "message";

        public void Initialize(string channelId, string channelName, string channelDescription, int pendingIntentId)
        {
            this._channelId = channelId;
            this._channelName = channelName;
            this._channelDescription = channelDescription;
            this._pendingIntentId = pendingIntentId;

            try
            {
                CreateNotificationChannel();
            }
            catch
            {
                _channelInitialized = false;
                return;
            }

            _channelInitialized = true;
        }
        private void CreateNotificationChannel()
        {
            _notificationManager = (NotificationManager)Application.Context.GetSystemService(Application.NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(_channelName);
                var channel = new NotificationChannel(_channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = _channelDescription,
                };
                _notificationManager.CreateNotificationChannel(channel);
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }

        Random rand = new Random();
        public async Task<int> ScheduleNotification(string title, string message, bool soundOn = true, bool vibrateOn = true)
        {
            if (!_channelInitialized)
            {
                return -1;
            }

            _messageId = rand.Next(int.MaxValue);

            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);

            var pendingIntent = PendingIntent.GetActivity(Application.Context, _pendingIntentId, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context, _channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(await BitmapFactory.DecodeResourceAsync(Application.Context.Resources, Resource.Drawable.xamagonBlue))
                .SetSmallIcon(Resource.Drawable.xamagonBlue)
                .SetAutoCancel(true);

            int defaultSetting = 0;
            if (soundOn)
                defaultSetting |= (int)NotificationDefaults.Sound;
            if (vibrateOn)
                defaultSetting |= (int)NotificationDefaults.Vibrate;
            builder.SetDefaults(defaultSetting);

            var notification = builder.Build();
            _notificationManager.Notify(_messageId, notification);

            return _messageId;
        }
    }
}