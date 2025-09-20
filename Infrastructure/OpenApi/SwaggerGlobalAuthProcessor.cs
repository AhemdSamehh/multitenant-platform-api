using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.OpenApi
{
    public class SwaggerGlobalAuthProcessor(string schema) : IOperationProcessor
    {
        private readonly string _schema = schema;
        public SwaggerGlobalAuthProcessor():this(JwtBearerDefaults.AuthenticationScheme) 
        {
            
        }
        public bool Process(OperationProcessorContext context)
        {
            IList<object> list = ((AspNetCoreOperationProcessorContext)context)
                .ApiDescription.ActionDescriptor.TryGetPropertyValue<List<object>>("EndpointMetadata");

            if(list is not null)
            {
                if(list.OfType<AllowAnonymousAttribute>().Any())
                {
                    return true;
                }
                if(context.OperationDescription.Operation.Security.Count == 0)
                {
                    (context.OperationDescription.Operation.Security ??= [])
                        .Add(new OpenApiSecurityRequirement
                        {
                            {
                                _schema,
                                Array.Empty<string>()
                            }
                        });     
                }
            }
            return true;
        }
    }

    public static class ObjectExtensions
    {
        public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue = default) =>
            obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo ? (T)propertyInfo.GetValue(obj) : defaultValue;
    }
}
