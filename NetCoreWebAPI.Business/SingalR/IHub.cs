using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business.SingalR
{
    public interface IHub
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessage(string message);
    }
}
