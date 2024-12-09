using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using udi.Device_Controller_SignalR.Hubs;
using udi.Device_Controller_SignalR.Interface.DeviceController;
using udi.Device_Controller_SignalR.Model.DeviceController;
using udi.Device_Controller_SignalR.Model.DyanmicModel;
using udi.Device_Controller_SignalR.Model.Other;

namespace udi.Device_Controller_SignalR.Service.DeviceControoler
{
    public class GetDeviceContrlService : IGetDeviceControlService
    {
        private readonly IHubContext<SignalServer> _context;
        internal static SqlDependency dependencyDevice = null;
        private string _connectionStr = "";


        public GetDeviceContrlService(IHubContext<SignalServer> context)
        {
            _context = context;
        }

        public async Task<DyanmicModel<List<DeviceControlModel>>> GetDeciveControl(string connectionStr)
        {
            _connectionStr = connectionStr;
            DyanmicModel<List<DeviceControlModel>> dyanmicModel = new DyanmicModel<List<DeviceControlModel>>();
            List<DeviceControlModel> deviceControls = new List<DeviceControlModel>();

            string sqlStr = @"SELECT DEVICE_ID
                                 ,ORDER_ID    
                                 ,STATE
                                 ,DESCRIPTION
                                 ,B_TIME
                                 ,E_TIME
                                 ,STATE_DESCRIPTION
                                 ,TASK_ID
                                 ,BREATHING_ORDER
                                 ,BREATHING_LIGHT
                                 ,BREATHING_ALARM
                                 ,WO_ASSIGN
                                 ,WO_RESULT
                                 ,WO_RATE
                                 ,QTY_ASSIGN
                                 ,QTY_RESULT
                                 ,QTY_RATE
                             FROM dev.UDI_CONTROL";
                           

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStr))
                using (SqlCommand com = new SqlCommand(sqlStr, sqlConnection))
                {
                    sqlConnection.Open();
                    SqlDependency.Start(_connectionStr);


                    //if (dependencyDevice != null)
                    //{
                    //    dependencyDevice.OnChange -= dbChangeNotification;
                    //    dependencyDevice.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                    //}

                    if (dependencyDevice == null)
                    {
                        dependencyDevice = new SqlDependency(com);
                        dependencyDevice.OnChange -= dbChangeNotification;
                        dependencyDevice.OnChange += new OnChangeEventHandler(dbChangeNotification);
                    }


                    using (SqlDataReader reader = await com.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            DeviceControlModel deviceControlModel = new DeviceControlModel
                            {
                                DEVICE_ID = reader["DEVICE_ID"].ToString(),
                                ORDER_ID = reader.IsDBNull(reader.GetOrdinal("ORDER_ID")) ? null : (int)reader["ORDER_ID"],
                                STATE = reader.IsDBNull(reader.GetOrdinal("STATE")) ? null : (byte)reader["STATE"],
                                DESCRIPTION = reader["DESCRIPTION"].ToString(),
                                STATE_DESCRIPTION = reader["STATE_DESCRIPTION"].ToString(),
                                TASK_ID = reader["TASK_ID"].ToString(),
                                BREATHING_ORDER = reader.IsDBNull(reader.GetOrdinal("BREATHING_ORDER")) ? null : (byte)reader["BREATHING_ORDER"],
                                BREATHING_LIGHT = reader["BREATHING_LIGHT"].ToString(),
                                BREATHING_ALARM = reader["BREATHING_ALARM"].ToString(),
                                WO_ASSIGN = (int)reader["WO_ASSIGN"],
                                WO_RESULT = (int)reader["WO_RESULT"],
                                WO_RATE = reader["WO_RATE"].ToString(),
                                QTY_ASSIGN = (int)reader["QTY_ASSIGN"],
                                QTY_RESULT = (int)reader["QTY_RESULT"],
                                QTY_RATE = reader["QTY_RATE"].ToString(),
                                B_TIME = reader["B_TIME"].ToString(),
                                E_TIME = reader["E_TIME"].ToString(),
                                RTN_CODE = 0
                            };

                            deviceControls.Add(deviceControlModel);
                        }

                    }
                    dyanmicModel.DATA = deviceControls;
                }
            }
            catch (Exception ex)
            {
                RtmMsg rtmMsg = new RtmMsg
                {
                    RTN_CODE = 1,
                    RTN_MSG = ex.Message,
                };

                dyanmicModel.rtmMsg = rtmMsg;
            }

            return dyanmicModel;
        }

        // 修改此方法的返回类型为 void
        public void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {

            if (dependencyDevice != null)
            {
                dependencyDevice.OnChange -= dbChangeNotification;
                dependencyDevice.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                dependencyDevice = null;
            }


            if (e.Type == SqlNotificationType.Change)
            {
                Task.Run(async () =>
                {
                    dependencyDevice = null;
                    DyanmicModel<List<DeviceControlModel>> dyanmicModel = await GetDeciveControl(_connectionStr).ConfigureAwait(false);
                    await _context.Clients.All.SendAsync("refreshDeviceControl", JsonConvert.SerializeObject(dyanmicModel.DATA)).ConfigureAwait(false);
                });
            }
        }
    }
}
