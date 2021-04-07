using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Common.AutoMapper
{
    public static class AtuoMapperConfig
    {
        public static MapperConfigurationExpression ConfigurationExpression(this MapperConfigurationExpression expression)
        {

            return expression;
        }
    }
}
