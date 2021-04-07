using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Common.AutoMapper
{
    public sealed class AutoMapToAttribute : Attribute
    {
        public Type TargetType { get; }

        public bool ReverseMap { get; }

        /// <summary>
        /// 自动映射特性标签
        /// </summary>
        /// <param name="targetType">映射目标类型</param>
        /// <param name="reverseMap">是否反向映射，默认true</param>
        public AutoMapToAttribute(Type targetType, bool reverseMap = true)
        {
            TargetType = targetType;
            ReverseMap = reverseMap;
        }
    }
}
