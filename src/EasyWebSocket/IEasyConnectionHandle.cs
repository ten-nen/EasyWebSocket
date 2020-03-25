using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public interface IEasyConnectionHandle
    {
        void OnOpen();
        void OnClose();
        void OnError(Exception e);
    }
}
