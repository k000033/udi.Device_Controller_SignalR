using udi.Device_Controller_SignalR.Model.DeviceController;
using udi.Device_Controller_SignalR.Model.DyanmicModel;

namespace udi.Device_Controller_SignalR.Interface.DeviceController
{
    public interface IGetDeviceControlService
    {

        public Task<DyanmicModel<List<DeviceControlModel>>> GetDeciveControl( string connectionStr);
      
    }
}
