using System;
using Newtonsoft.Json.Serialization;

namespace Pubquiz.WebApi.Helpers
{
    public class DiCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DiCamelCasePropertyNamesContractResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var result = base.CreateObjectContract(objectType);

            var instance = _serviceProvider.GetService(objectType);
            if (instance != null)
            {
                result.DefaultCreator = () => _serviceProvider.GetService(objectType);
            }

            return result;
        }

        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            
            JsonDictionaryContract contract = base.CreateDictionaryContract(objectType);

            contract.DictionaryKeyResolver = propertyName => propertyName;

            return contract;
        }
    }
}