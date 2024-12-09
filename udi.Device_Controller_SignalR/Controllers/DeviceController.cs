using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using udi.Device_Controller_SignalR.Interface.DBConn;
using udi.Device_Controller_SignalR.Interface.DeviceController;

namespace udi.Device_Controller_SignalR.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeviceController : Controller
    {

        private readonly IDBConn _dbConn;
        private readonly IGetDeviceControlService _deviceControl;
        private readonly IGetUdiBreathing _getUdiBreathing;
        private readonly IGetRecycleProgress _getUdiRecycleProgress;

        public DeviceController(IDBConn dbConn, IGetDeviceControlService deviceControl, IGetUdiBreathing getUdiBreathing,IGetRecycleProgress getRecycleProgress)
        {
            _dbConn = dbConn;
            _deviceControl = deviceControl;
            _getUdiBreathing = getUdiBreathing;
            _getUdiRecycleProgress = getRecycleProgress;    
        }

        [HttpGet]
        public async Task<IActionResult> getDeviceControl()
        {
            string connectionStr = _dbConn.GetConnectionStr("UDI");
            var res = await  _deviceControl.GetDeciveControl(connectionStr);

            if (res.rtmMsg != null)
            {
                return Ok(res.rtmMsg);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));
        }

        [HttpGet]
        public IActionResult getUdiBreathing()
        {
            string connectionStr = _dbConn.GetConnectionStr("UDI");
            var res = _getUdiBreathing.getUdiBreathing(connectionStr);
            if (res.rtmMsg != null)
            {
                return Ok(res.rtmMsg);
            }

            return Ok(JsonConvert.SerializeObject(res.DATA));
        }

        [HttpGet]
        public IActionResult getRecycle()
        {
            string connection = _dbConn.GetConnectionStr("UDI");
            var res = _getUdiRecycleProgress.GetRecyleProgress(connection);
            if (res.rtmMsg != null)
            {
                return Ok(res.rtmMsg);
            }
            return Ok(JsonConvert.SerializeObject(res.DATA));
        }

    }
}
