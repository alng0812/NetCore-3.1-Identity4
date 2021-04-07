using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace NetCoreWebAPI.Common.AutoMapper
{
    public static class AutoMapperExtension
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection service)
        {
            var factory = new AutoInjectFactory();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            factory.AddAssemblys(assemblys);
            service.TryAddSingleton(factory);

            service.TryAddSingleton<MapperConfigurationExpression>();
            service.TryAddSingleton(serviceProvider =>
            {
                var mapperConfigurationExpression = serviceProvider.GetRequiredService<MapperConfigurationExpression>();
                //mapperConfigurationExpression.ConfigurationExpression();

                var factory = serviceProvider.GetRequiredService<AutoInjectFactory>();

                foreach (var (sourceType, targetType, reverseMap) in factory.ConvertList)
                {
                    if (reverseMap)
                        mapperConfigurationExpression.CreateMap(sourceType, targetType).ReverseMap();
                    else
                        mapperConfigurationExpression.CreateMap(sourceType, targetType);
                }

                var instance = new MapperConfiguration(mapperConfigurationExpression);

                //instance.AssertConfigurationIsValid();

                return instance;
            });

            service.TryAddSingleton(serviceProvider =>
            {
                var mapperConfiguration = serviceProvider.GetRequiredService<MapperConfiguration>();

                return mapperConfiguration.CreateMapper();
            });

            return service;
        }

        public static IMapperConfigurationExpression UseAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.ApplicationServices.GetRequiredService<MapperConfigurationExpression>();
        }
        public static void UseMapperAutoInject(this IApplicationBuilder applicationBuilder)
        {
            var factory = applicationBuilder.ApplicationServices.GetRequiredService<AutoInjectFactory>();
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            factory.AddAssemblys(assemblys);
        }
    }
}
