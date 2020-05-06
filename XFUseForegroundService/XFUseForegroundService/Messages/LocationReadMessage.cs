using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFUseForegroundService.Messages
{
    public class LocationReadMessage
    {
        public IGpsReading GpsInfo { get; internal set; }
    }
}
