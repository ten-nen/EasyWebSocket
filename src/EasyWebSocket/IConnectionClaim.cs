using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebSocket
{
    public interface IConnectionClaim
    {
        string Id { get; set; }
        string Name { get; set; }
        string Role { get; set; }
    }

}
