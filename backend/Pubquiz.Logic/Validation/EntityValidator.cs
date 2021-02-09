using System;
using System.Linq;
using System.Reflection;
using MediatR;
using Pubquiz.Domain;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Validation
{
    public class EntityValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequest _request;
        private readonly Type _requestType;

        public EntityValidator(IUnitOfWork unitOfWork, IRequest request)
        {
            _unitOfWork = unitOfWork;
            _request = request;
            _requestType = request.GetType();
        }

        public void CheckValidationAttributes()
        {
            var attributes = Attribute.GetCustomAttributes(_requestType).OfType<ValidateEntityAttribute>();

            foreach (var attribute in attributes)
            {
                var m = GetType().GetMethod("CheckEntity", BindingFlags.NonPublic | BindingFlags.Instance);
                var genericCheckEntityMethod = m!.MakeGenericMethod(attribute.EntityType);
                genericCheckEntityMethod.Invoke(this,
                    new object[] {attribute.IdPropertyName, ResultCode.InvalidEntityId});
            }
        }

        private void CheckEntity<TEntity>(string entityIdPropertyName, ResultCode resultCode)
            where TEntity : Model, new()
        {
            // get the property or field
            var property = _requestType.GetProperty(entityIdPropertyName);
            var field = _requestType.GetField(entityIdPropertyName);

            if (property == null && field == null)
            {
                throw new Exception(
                    $"Could not find property or field {entityIdPropertyName} on {_requestType.Name}");
            }

            var value = property?.GetValue(_request) ?? field?.GetValue(_request);
            if (value == null)
            {
                throw new Exception(
                    $"Could not get the property or field value of {entityIdPropertyName} on {_requestType.Name}");
            }

            var entityId = (string) value;
            var entityCollection = _unitOfWork.GetCollection<TEntity>();
            var entity = entityCollection.GetAsync(entityId).Result;
            if (entity == null)
            {
                throw new DomainException(resultCode, $"Invalid {entityIdPropertyName}.", true);
            }
        }
    }
}