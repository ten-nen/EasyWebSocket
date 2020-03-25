using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public class EasyConnectionCollection
    {
        internal IEnumerable<IEasyConnection> Connections { get; }
        public EasyConnectionCollection(IEnumerable<IEasyConnection> connections)
        {
            Connections = connections;
        }

        public async Task CloseAsync()
        {
            await Task.Run(() =>
            {
                foreach (var connection in Connections)
                {
                    connection.CloseAsync();
                }
            });
        }

        public async Task SendAsync(string method, params object[] messages)
        {
            await Task.Run(() =>
            {
                foreach (var connection in Connections)
                {
                    connection.SendAsync(method, messages);
                }
            });
        }
    }
}
