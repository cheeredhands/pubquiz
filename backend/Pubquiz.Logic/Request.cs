using System;
using System.Linq;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic
{
    public abstract class Request
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Request(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected void CheckValidationAttributes()
        {
            var attributes = Attribute.GetCustomAttributes(GetType()).OfType<ValidateEntityAttribute>();

            foreach (var attribute in attributes)
            {
                if (attribute.EntityType == typeof(Team))
                {
                    CheckEntity<Team>(attribute.IdPropertyName, ErrorCodes.InvalidTeamId);
                }

                if (attribute.EntityType == typeof(User))
                {
                    CheckEntity<User>(attribute.IdPropertyName, ErrorCodes.InvalidUserId);
                }

                if (attribute.EntityType == typeof(Game))
                {
                    CheckEntity<Game>(attribute.IdPropertyName, ErrorCodes.InvalidGameId);
                }

                if (attribute.EntityType == typeof(Question))
                {
                    CheckEntity<Question>(attribute.IdPropertyName, ErrorCodes.InvalidQuestionId);
                }
            }
        }

        private void CheckEntity<TEntity>(string entityIdPropertyName, int errorCode) where TEntity : Model, new()
        {
            // get the property or field
            var property = GetType().GetProperty(entityIdPropertyName);
            var field = GetType().GetField(entityIdPropertyName);

            if (property == null && field == null)
            {
                throw new Exception(
                    $"Could not find property or field {entityIdPropertyName} on {GetType().Name}");
            }

            var value = property?.GetValue(this) ?? field?.GetValue(this);
            if (value == null)
            {
                throw new Exception(
                    $"Could not get the property or field value of {entityIdPropertyName} on {GetType().Name}");
            }

            var entityId = (Guid) value;
            var entityCollection = UnitOfWork.GetCollection<TEntity>();
            var entity = entityCollection.GetAsync(entityId).Result;
            if (entity == null)
            {
                throw new DomainException(errorCode, $"Invalid {entityIdPropertyName}.", true);
            }
        }
    }
}