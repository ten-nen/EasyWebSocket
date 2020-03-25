using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public abstract class EasyConnectionHandle : IEasyConnectionHandle
    {
        public EasyConnectionCollection AllConnections;
        public EasyConnection Current;
        public abstract void OnClose();

        public abstract void OnError(Exception e);

        public abstract void OnOpen();
    }
}
