using Shiny;
using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFUseForegroundService.Messages;

namespace XFUseForegroundService.Services
{
    public class LocationTask
    {
        private INotificationManager _notificationManager;
        private IGpsManager _gpsManager;

        public LocationTask()
        {
            _notificationManager = DependencyService.Get<INotificationManager>();
            _notificationManager.Initialize("foreground2", "foreground2", "this is channel", 0);
            _gpsManager = ShinyHost.Resolve<IGpsManager>();
        }

        private IDisposable _gpsObserver = null;
        public async Task Run(CancellationToken token)
        {
            if (_gpsManager.IsListening)
                return;

            #region Subscribe GPS reading event
            //購読重複を避けるために購読を解除する
            _gpsObserver?.Dispose();

            //位置情報取得イベントを購読
            _gpsObserver = _gpsManager
                .WhenReading()
                .Subscribe(async x =>
                {
                    if(token.IsCancellationRequested)
                    {
                        await _gpsManager.StopListener();
                        _gpsObserver?.Dispose();
                        return;
                    }
                    MainThread.BeginInvokeOnMainThread(()=> {
                        var message = new LocationReadMessage
                        {
                            GpsInfo = x,
                        };
                        MessagingCenter.Send(message, nameof(LocationReadMessage));
                    });
                    await _notificationManager.ScheduleNotification($"{x.Position.Latitude}, {x.Position.Longitude}", "test");
                });
            #endregion

            #region GPS start listen.
            //位置情報取得許可を得る
            var checkResult = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (checkResult != PermissionStatus.Granted)
            {
                var requestResult = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (requestResult != PermissionStatus.Granted)
                    return;
            }

            var request = new GpsRequest
            {
                Interval = TimeSpan.FromSeconds(5),
                UseBackground = false,
            };
            await _gpsManager.StartListener(request);
            #endregion
        }
    }
}
