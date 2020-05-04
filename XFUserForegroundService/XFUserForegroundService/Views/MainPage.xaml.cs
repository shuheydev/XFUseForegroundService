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
        public MainPage()
        {
            InitializeComponent();

            _notificationManager = DependencyService.Get<INotificationManager>();
            _notificationManager.Initialize("default","default","this is channel",0);
            _notificationManager.NotificationReceived += _notificationManager_NotificationReceived;
        }

        private void _notificationManager_NotificationReceived(object sender, EventArgs e)
        {
            var eventData = (NotificationEventArgs)e;
            ShowNotification(eventData.Title, eventData.Message);
        }
        private void ShowNotification(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(() => {
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
    }
}
