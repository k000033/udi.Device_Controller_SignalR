using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace udi.Device_Controller_SignalR.Hubs
{
    public class SignalServer : Hub
    {
        private static readonly ConcurrentDictionary<string, SqlDependency> _dependencies = new ConcurrentDictionary<string, SqlDependency>();
        private static readonly ConcurrentDictionary<string, bool> _connectedClients = new ConcurrentDictionary<string, bool>();
        private static readonly object _dependencyLock = new object();

        /// 連線事件
        public override async Task OnConnectedAsync()
        {
            _connectedClients.TryAdd(Context.ConnectionId, true);

            // 发送用户数更新
            await Clients.All.SendAsync("UpdateUserCount", _connectedClients.Count);
            await base.OnConnectedAsync();
        }

        /// 離線事件
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedClients.TryRemove(Context.ConnectionId, out _);
            //StopSqlDependency(Context.ConnectionId);
            // 发送用户数更新
            await Clients.All.SendAsync("UpdateUserCount", _connectedClients.Count);
            await base.OnDisconnectedAsync(exception);
        }

        // 停止監聽，不會有數據更改通知
        //private void StopSqlDependency(string connectionId)
        //{
        //    lock (_dependencyLock)
        //    {
        //        if (_dependencies.TryRemove(connectionId, out var dependency))
        //        {
        //            SqlDependency.Stop(_connectionStr); // 停止 SQL 依赖
        //        }
        //    }
        //}

        public int GetConnectedClientsCount()
        {
            return _connectedClients.Count;
        }



    }
}
