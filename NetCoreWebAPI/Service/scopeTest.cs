using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Service
{
    public class scopeTest : IScopeService
    {
        private string guid;

        public scopeTest()
        {
            guid = $"时间:{DateTime.Now}, guid={ Guid.NewGuid()}";
        }

        public override string ToString()
        {
            return guid;
        }
    }

    public interface IScopeService
    {

    }
}
