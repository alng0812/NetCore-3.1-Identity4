using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Common.AutoMapper
{
    public class AutoInjectFactory
    {
        public List<(Type, Type, bool)> ConvertList { get; } = new List<(Type, Type, bool)>();

        public void AddAssemblys(params Assembly[] assemblys)
        {
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes()
                    .Where(_type => _type.GetCustomAttribute<AutoMapToAttribute>() != null);
                foreach (var type in types)
                {
                    var atribute = type.GetCustomAttribute<AutoMapToAttribute>();
                    ConvertList.Add((type, atribute.TargetType, atribute.ReverseMap));
                }
            }
        }
    }
}
