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
    public class GetUdiBreathing : IGetUdiBreathing
    {
        private readonly IHubContext<SignalServer> _context;
        internal static SqlDependency dependencyBreathing = null;
        private string _connectionStr = "";


        public GetUdiBreathing(IHubContext<SignalServer> context)
        {
            _context = context;
            //SqlDependency.Start(_connectionStr);
        }
        public DyanmicModel<UdiBreathingModel> getUdiBreathing(string connectionStr)
        {
            _connectionStr = connectionStr;
            //_connectionStr = connectionStr;
            DyanmicModel<UdiBreathingModel> dyanmicModel = new DyanmicModel<UdiBreathingModel>();
            UdiBreathingModel udiBreathing = new UdiBreathingModel();


            string sqlStr = @"SELECT BREATHING FROM [dbo].[UDI_PROFILE] ";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                using (SqlCommand sqlCommand = new SqlCommand(sqlStr, connection))
                {
                    connection.Open();
                    SqlDependency.Start(_connectionStr);



                    if (dependencyBreathing != null)
                    {
                        dependencyBreathing.OnChange -= dbChangeNotification;
                        dependencyBreathing.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                    }

                    if (dependencyBreathing == null)
                    {
                        dependencyBreathing = new SqlDependency(sqlCommand);
                        dependencyBreathing.OnChange -= dbChangeNotification;
                        dependencyBreathing.OnChange += new OnChangeEventHandler(dbChangeNotification);
                    }


                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            udiBreathing.BREATHING = reader["BREATHING"].ToString();
                        }
                    }
                    dyanmicModel.DATA = udiBreathing;
                };
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



        public void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {

            if (dependencyBreathing != null)
            {
                dependencyBreathing.OnChange -= dbChangeNotification;
                dependencyBreathing.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                dependencyBreathing = null;
            }


            if (e.Type == SqlNotificationType.Change)
            {

                // 调用异步方法时不等待其结果
                Task.Run(async () =>
                {
                    dependencyBreathing = null;
                    DyanmicModel<UdiBreathingModel> dyanmicModel = getUdiBreathing(_connectionStr);
                    await _context.Clients.All.SendAsync("refreshUdiBreathing", JsonConvert.SerializeObject(dyanmicModel.DATA));
                });
            }

        }
    }
}
