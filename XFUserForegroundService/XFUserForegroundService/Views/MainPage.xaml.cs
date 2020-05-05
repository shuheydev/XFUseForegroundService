using Shiny;
using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFUserForegroundService.Services;

namespace XFUserForegroundService
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        INotificationManager _notificationManager;
        IGpsManager _gpsManager;

        public MainPage()
        {
            InitializeComponent();

            _notificationManager = DependencyService.Get<INotificationManager>();
            _notificationManager.Initialize("default", "default", "this is channel", 0);
            _notificationManager.NotificationReceived += _notificationManager_NotificationReceived;

            _gpsManager = ShinyHost.Resolve<IGpsManager>();
        }

        private void _notificationManager_NotificationReceived(object sender, EventArgs e)
        {
            var eventData = (NotificationEventArgs)e;
            ShowNotification(eventData.Title, eventData.Message);
        }
        private void ShowNotification(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                stackLayout.Children.Add(msg);
            });
        }

        private async void Notify_Clicked(object sender, EventArgs e)
        {
            await _notificationManager.ScheduleNotification($"Test Notification : {DateTimeOffset.Now}", "Test Message");
        }

        private IDisposable _gpsObserver;
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (_gpsManager.IsListening)
                return;

            _gpsObserver?.Dispose();

            _gpsObserver = _gpsManager
                .WhenReading()
                .Subscribe(async x =>
                {
                    await _notificationManager.ScheduleNotification($"{x.Position.Latitude}, {x.Position.Longitude}", "test");
                });

            var checkResult=await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if(checkResult!=PermissionStatus.Granted)
            {
                var requestResult = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (requestResult != PermissionStatus.Granted)
                    return;
            }

            var request = new GpsRequest
            {
                Interval = TimeSpan.FromSeconds(5),
                UseBackground = true,
            };
            await _gpsManager.StartListener(request);
        }
    }
}
