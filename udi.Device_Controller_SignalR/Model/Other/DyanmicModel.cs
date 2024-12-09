using udi.Device_Controller_SignalR.Model.Other;

namespace udi.Device_Controller_SignalR.Model.DyanmicModel
{
    public class DyanmicModel<T>
    {

        public T? DATA { get; set; }
        public RtmMsg? rtmMsg { get; set; }
    }
}
