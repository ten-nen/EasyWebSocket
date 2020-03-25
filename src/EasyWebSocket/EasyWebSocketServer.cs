using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace EasyWebSocket
{
    public class EasyWebSocketServer : IDisposable
    {
        private EasyWebSocketServer() { }
        private WebSocketServer _server;
        private FlashPoliyServer _flashPoliyServer;
        private static EasyWebSocketServer _instance;
        private Dictionary<string, ConnectionHandleBuilder> _dicHandleBuilder = new Dictionary<string, ConnectionHandleBuilder>();
        private List<EasyConnection> _connections = new List<EasyConnection>();
        private EasyWebSocketServerSetting _serverSetting = new EasyWebSocketServerSetting();
        public static EasyWebSocketServer CreateServer(Action<EasyWebSocketServerSetting> configureServer = null)
        {
            if (_instance == null)
                _instance = new EasyWebSocketServer();
            if (configureServer != null)
                configureServer(_instance._serverSetting);

            _instance._server = new WebSocketServer($"ws://{_instance._serverSetting.IP}:{_instance._serverSetting.Port}");
            _instance._server.RestartAfterListenError = true;
            return _instance;
        }

        public EasyWebSocketServer MapHandle<T>(string path) where T : EasyConnectionHandle, new()
        {
            var lowerPath = path.ToLower();
            if (_instance._dicHandleBuilder.Keys.Contains(lowerPath))
                _instance._dicHandleBuilder[lowerPath] = new ConnectionHandleBuilder(typeof(T));
            else
                _instance._dicHandleBuilder.Add(lowerPath, new ConnectionHandleBuilder(typeof(T)));
            return _instance;
        }
        public EasyWebSocketServer WithFlashPoliy(Action<FlashPoliyServerSetting> configureFlashPoliyServer=null)
        {
            _instance._serverSetting.FlashPoliyServerSetting = new FlashPoliyServerSetting();
            if (configureFlashPoliyServer != null)
                configureFlashPoliyServer(_instance._serverSetting.FlashPoliyServerSetting);
            _instance._flashPoliyServer = new FlashPoliyServer(_instance._serverSetting.FlashPoliyServerSetting);
            return _instance;
        }

        public void Start()
        {
            if (_instance._flashPoliyServer != null)
                _instance._flashPoliyServer.Start();
            _server.Start(connection =>
            {
                var path = connection.ConnectionInfo.Path.ToLower();
                if (_instance._dicHandleBuilder.Keys.Contains(path))
                {
                    var handleBuilder = _instance._dicHandleBuilder[path];
                    if (handleBuilder != null)
                    {
                        var handle = handleBuilder.Build();
                        handle.Current = new EasyConnection(connection);
                        handle.AllConnections = new EasyConnectionCollection(_connections);
                        connection.OnOpen = () =>
                        {
                            _connections.Add(new EasyConnection(connection));
                            handle.Current.Status = ConnectionStatus.Opened;
                            handle.OnOpen();
                        };
                        connection.OnClose = () =>
                        {
                            _connections.Remove(handle.Current);
                            handle.Current.Status = ConnectionStatus.Closed;
                            handle.OnClose();
                        };
                        connection.OnError = handle.OnError;
                        connection.OnMessage = message =>
                        {
                            try
                            {
                                var json = JObject.Parse(message);

                                var method = json["method"].Value<string>();
                                if (string.IsNullOrWhiteSpace(method))
                                    return;
                                
                                var methods = handleBuilder.Methods.Where(v => v.Name.Equals(method, StringComparison.OrdinalIgnoreCase));
                                if (methods.Count() == 0)
                                    return;
                                
                                MethodInfo methodInfo = methods.FirstOrDefault(v => v.GetParameters().Length == json["data"].Count());
                                if (methodInfo == null)
                                    return;
                                var parameters = methodInfo.GetParameters();
                                var agrs = new List<object>();
                                for (int i = 0; i < parameters.Length; i++)
                                {
                                    object objArg;
                                    var argStr = json["data"][i].ToString().Trim();
                                    if ((argStr.StartsWith("{") && argStr.EndsWith("}")) || (argStr.StartsWith("[") && argStr.EndsWith("]")))
                                    {
                                        objArg = JsonConvert.DeserializeObject(argStr, parameters[i].ParameterType);
                                    }
                                    else
                                    {
                                        var converter = System.ComponentModel.TypeDescriptor.GetConverter(parameters[i].ParameterType);
                                        objArg = converter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, argStr);
                                    }
                                    agrs.Add(objArg);
                                }
                                try
                                {
                                    methodInfo.Invoke(handle, agrs.ToArray());
                                }
                                catch (Exception ex)
                                {
                                    if (ex.InnerException != null)
                                        throw ex.InnerException;
                                    throw ex;
                                }
                            }
                            catch (Exception e)
                            {
                                handle.OnError(e);
                            }
                        };
                    }
                }
            });
        }

        public void Close()
        {
            _flashPoliyServer.Close();
            _connections.ForEach(connection =>
            {
                connection.CloseAsync().Wait();
            });
        }

        public void Dispose()
        {
            _flashPoliyServer.Dispose();
            _server.Dispose();
        }
    }

    public class EasyWebSocketServerSetting
    {
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 12345;
        internal FlashPoliyServerSetting FlashPoliyServerSetting;
    }
}
