using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFUserForegroundService.Messages
{
    public class LocationReadMessage
    {
        public IGpsReading GpsInfo { get; internal set; }
    }
}
