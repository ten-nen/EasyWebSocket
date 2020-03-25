using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    internal sealed class ConnectionHandleBuilder
    {
        private Type _handleType { get; set; }
        internal MethodInfo[] Methods { get; set; }
        internal ConnectionHandleBuilder(Type handleType)
        {
            _handleType = handleType;
            Methods = handleType.GetMethods();
            if (Methods.GroupBy(v => new { Name = v.Name, ArgsLen = v.GetParameters() }).Any(v => v.Count() > 1))
                throw new ArgumentException(handleType.FullName + ":methods with the same name cannot have the same number of parameters");
        }

        internal EasyConnectionHandle Build()
        {
            var handle = (EasyConnectionHandle)Activator.CreateInstance(_handleType);
            return handle;
        }
    }
}
