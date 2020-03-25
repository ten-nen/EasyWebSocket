using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    internal class FlashPoliyServer : IDisposable
    {
        private Socket flashPoliyListener;
        private FlashPoliyServerSetting _setting;
        internal FlashPoliyServer(FlashPoliyServerSetting setting)
        {
            _setting = setting;
            IPEndPoint remotePoint = new IPEndPoint(IPAddress.Parse(_setting.IP), _setting.Port);

            flashPoliyListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            flashPoliyListener.Bind(remotePoint);
            flashPoliyListener.Listen(setting.BackLog);
        }
        internal void Start()
        {
            Task.Run(action: Listen);
        }

        private void Listen()
        {
            while (true)
            {
                var socketHandle = flashPoliyListener.Accept();
                Task.Run(() =>
                {
                    var buffer = new byte[24];
                    var bytes = socketHandle.Receive(buffer);
                    if (bytes > 0)
                    {
                        var data = Encoding.ASCII.GetString(buffer, 0, bytes);
                        if (data.Contains(_setting.FlashProtocol))
                        {
                            buffer = Encoding.ASCII.GetBytes(_setting.PolicyXml.ToCharArray());
                            socketHandle.Send(buffer, buffer.Length, 0);
                        }
                    }
                    socketHandle.Shutdown(SocketShutdown.Both);
                    socketHandle.Close();
                });
            }
        }

        internal void Close()
        {
            if (flashPoliyListener != null)
                flashPoliyListener.Close();
        }
        public void Dispose()
        {
            if (flashPoliyListener != null)
                flashPoliyListener.Dispose();
        }
    }

    public class FlashPoliyServerSetting
    {
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 843;
        public string PolicyXml { get; set; } = "<?xml version=\"1.0\"?><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0";
        public int BackLog { get; set; } = 10;
        public string FlashProtocol { get; set; } = "policy-file-request";
    }

}
