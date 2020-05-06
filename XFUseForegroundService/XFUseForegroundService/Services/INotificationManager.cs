using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XFUseForegroundService.Services
{
    public interface INotificationManager
    {
        event EventHandler NotificationReceived;

        void Initialize(string channelId, string channelName, string channelDescription, int pendingIntentId);

        Task<int> ScheduleNotification(string title, string message, bool soundOn=true, bool vibrateOn=true);
        void ReceiveNotification(string title, string message);
    }

    public class NotificationEventArgs:EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
