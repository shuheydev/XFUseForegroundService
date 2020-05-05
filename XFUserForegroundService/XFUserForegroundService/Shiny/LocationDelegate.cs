using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XFUserForegroundService.Shiny
{
    public class LocationDelegate : IGpsDelegate
    {

        public Task OnReading(IGpsReading reading)
        {
            throw new NotImplementedException();
        }
    }
}
