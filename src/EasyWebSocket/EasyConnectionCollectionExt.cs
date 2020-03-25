using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public static class EasyConnectionCollectionExt
    {
        public static EasyConnectionCollection Find(this EasyConnectionCollection source, Func<IEasyConnection, bool> predicate)
        {
            return new EasyConnectionCollection(source.Connections.Where(predicate));
        }

        public static EasyConnectionCollection Find(this EasyConnectionCollection source, Func<IConnectionClaim, bool> predicate)
        {
            return new EasyConnectionCollection(source.Connections.Where(v => v.User != null && predicate(v.User)));
        }
    }
}
