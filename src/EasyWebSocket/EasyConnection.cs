using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public class EasyConnection : IEasyConnection
    {
        private IWebSocketConnection _current;
        private ConnectionStatus _status;
        public ConnectionStatus Status
        {
            get
            {
                return _status;
            }
            internal set
            {
                _status = value;
            }
        }
        public IConnectionClaim User { get; } = new ConnectionUser();

        public string Path { get { return _current.ConnectionInfo.Path; } }
        public string Host { get { return _current.ConnectionInfo.Host; } }
        public string ClientIpAddress { get { return _current.ConnectionInfo.ClientIpAddress; } }
        public int ClientPort { get { return _current.ConnectionInfo.ClientPort; } }
        public IDictionary<string, string> Cookies { get { return _current.ConnectionInfo.Cookies; } }
        public IDictionary<string, string> Headers { get { return _current.ConnectionInfo.Headers; } }
        public string Origin { get { return _current.ConnectionInfo.Origin; } }
        public Guid Id { get { return _current.ConnectionInfo.Id; } }

        public EasyConnection(IWebSocketConnection connnection)
        {
            _current = connnection;
        }
        public async Task SendAsync(string method, params object[] messages)
        {
            await Task.Run(() =>
            {
                _current.Send(JsonConvert.SerializeObject(new { method = method, data = messages }));
            });
        }
        public async Task CloseAsync()
        {
            await Task.Run(() =>
            {
                _current.Close();
            });
        }
    }
}
