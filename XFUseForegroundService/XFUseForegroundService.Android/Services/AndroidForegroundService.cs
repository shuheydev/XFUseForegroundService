using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
//using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFUseForegroundService.Messages;
using XFUseForegroundService.Services;

namespace XFUseForegroundService.Droid.Services
{
    [Service]
    public class AndroidForegroundService : Service
    {
        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var locationTask = new LocationTask();
                    locationTask.Run(_cts.Token).Wait();
                }
                catch (Android.OS.OperationCanceledException)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        MainThread.BeginInvokeOnMainThread(() => MessagingCenter.Send(message, nameof(CancelledMessage)));
                    }
                }
            });

            var notification = CreateNotification();

            StartForeground(1, notification);

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }

            StopForeground(true);
            base.OnDestroy();
        }


        private Notification CreateNotification()
        {
            #region Create Channel
            string channelId = "foreground";
            string channelName = "foreground";
            string channelDescription = "The foreground channel for notifications";
            int _pendingIntentId = 1;

            NotificationManager _notificationManager;
            _notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Low)
                {
                    Description = channelDescription,
                };
                channel.EnableVibration(false);
                _notificationManager.CreateNotificationChannel(channel);
            }
            #endregion

            #region Create Notification
            Intent foregroundNotificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));

            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, _pendingIntentId, foregroundNotificationIntent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("ForegroundServiceApp")
                .SetContentText("Foreground service started")
                .SetOngoing(true)
                .SetColor(ActivityCompat.GetColor(Android.App.Application.Context, Resource.Color.colorAccent))
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.xamagonBlue))
                .SetSmallIcon(Resource.Drawable.xamagonBlue);

            var notification = builder.Build();
            #endregion

            return notification;
        }
    }
}