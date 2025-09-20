using Application.Pipelines;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class Startup
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddTransient(typeof(IPipelineBehavior<,>) , typeof(ValidationPipelineBehavior<,>))
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                });
        }

    }
}
