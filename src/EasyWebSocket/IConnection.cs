using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public interface IConnection
    {
        string ClientIpAddress { get; }
        int ClientPort { get; }
        IDictionary<string, string> Cookies { get; }
        IDictionary<string, string> Headers { get; }
        string Host { get; }
        Guid Id { get; }
        string Origin { get; }
        string Path { get; }
        Task SendAsync(string method, params object[] messages);
        Task CloseAsync();
    }
}
