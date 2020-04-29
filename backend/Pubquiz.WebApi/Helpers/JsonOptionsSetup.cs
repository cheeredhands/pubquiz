using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Pubquiz.WebApi.Helpers
{
    public class JsonOptionsSetup : IConfigureOptions<MvcNewtonsoftJsonOptions>
    {
        private readonly IServiceProvider _serviceProvider;

        public JsonOptionsSetup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(MvcNewtonsoftJsonOptions o)
        {
            o.SerializerSettings.ContractResolver =
                new DiCamelCasePropertyNamesContractResolver(_serviceProvider);
            o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        }
    }
}