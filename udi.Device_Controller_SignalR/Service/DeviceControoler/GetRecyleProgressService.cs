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
    public class GetRecyleProgressService: IGetRecycleProgress
    {


        private readonly IHubContext<SignalServer> _context;
        internal static SqlDependency dependencyRecyle = null;
        private string _connectionStr = "";


        public GetRecyleProgressService(IHubContext<SignalServer> context)
        {
            _context = context;
        }

        public DyanmicModel<RecyleProgressModel> GetRecyleProgress(string connectionStr)
        {
            _connectionStr = connectionStr;

            DyanmicModel<RecyleProgressModel> dyanmicModel = new DyanmicModel<RecyleProgressModel>();
            RecyleProgressModel recyleProgressModel = new RecyleProgressModel();


            string sqlStr = @"SELECT [RESULT_UPD_TIME]
                                    ,[RECYCLE_STATE]
                                    ,[RECYCLE_PROGRESS]
                                    ,[RECYCLE_PROGRESSING]
                                    ,[RECYCLE_MESSAGE]
                                    ,[RECYCLE_TIME]
                                FROM dbo.UDI_PROFILE";


            try
            {
                using (SqlConnection sqlCon = new SqlConnection(_connectionStr))
                using (SqlCommand sqlCommand = new SqlCommand(sqlStr, sqlCon))
                {
                    sqlCon.Open();
                    SqlDependency.Start(_connectionStr);


                    //if (dependencyRecyle != null)
                    //{
                    //    dependencyRecyle.OnChange -= dbChangeNotification;
                    //    dependencyRecyle.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                    //}

                    if (dependencyRecyle == null)
                    {
                        dependencyRecyle = new SqlDependency(sqlCommand);
                        dependencyRecyle.OnChange -= dbChangeNotification;
                        dependencyRecyle.OnChange += new OnChangeEventHandler(dbChangeNotification);
                    }


                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recyleProgressModel.RESULT_UPD_TIME = reader["RESULT_UPD_TIME"].ToString();
                            recyleProgressModel.RECYCLE_PROGRESS = (byte)reader["RECYCLE_PROGRESS"];
                            recyleProgressModel.RECYCLE_PROGRESSING = (byte)reader["RECYCLE_PROGRESSING"];
                            recyleProgressModel.RECYCLE_MESSAGE = reader["RECYCLE_MESSAGE"].ToString();
                            recyleProgressModel.RECYCLE_STATE = (byte)reader["RECYCLE_STATE"];
                            recyleProgressModel.RECYCLE_TIME = reader["RECYCLE_TIME"].ToString();
                        }

                        dyanmicModel.DATA = recyleProgressModel;
                    }
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


        public void dbChangeNotification(object sender, SqlNotificationEventArgs e)
        {

            if (dependencyRecyle != null)
            {
                dependencyRecyle.OnChange -= dbChangeNotification;
                dependencyRecyle.OnChange -= new OnChangeEventHandler(dbChangeNotification);
                dependencyRecyle = null;
            }


            if (e.Type == SqlNotificationType.Change)
            {

                // 调用异步方法时不等待其结果
                Task.Run(async () =>
                {
                    dependencyRecyle = null;
                    DyanmicModel<RecyleProgressModel> dyanmicModel = GetRecyleProgress(_connectionStr);
                   await _context.Clients.All.SendAsync("refreshRecycle", JsonConvert.SerializeObject(dyanmicModel.DATA));
                });
            }

        }
    }
}
