namespace udi.Device_Controller_SignalR.Model.DeviceController
{
    public class RecyleProgressModel
    {
        public string RESULT_UPD_TIME { get; set; }
        public byte RECYCLE_STATE { get; set; }
        public byte RECYCLE_PROGRESS { get; set; }
        public byte RECYCLE_PROGRESSING {get;set;}
        public string RECYCLE_MESSAGE { get; set; }
        public string RECYCLE_TIME { get; set; }
    }
}
