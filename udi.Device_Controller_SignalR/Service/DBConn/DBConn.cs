using udi.Device_Controller_SignalR.Interface.DBConn;

namespace udi.Device_Controller_SignalR.Service.DBConn
{
    public class DBConn : IDBConn
    {

        private readonly IConfiguration _configuration;

        public DBConn(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GetConnectionStr(string DBName)
        {
            string connectionString = _configuration[$"ConnectionStrings:{DBName}"];
            return connectionString;
        }
    }
}
