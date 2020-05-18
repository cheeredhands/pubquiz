using System;
using System.Linq;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;

namespace Pubquiz.Logic.Requests
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
                if (attribute.EntityType==typeof(Quiz))
                {
                    CheckEntity<Quiz>(attribute.IdPropertyName, ResultCode.InvalidQuizId);
                }
                if (attribute.EntityType == typeof(Team))
                {
                    CheckEntity<Team>(attribute.IdPropertyName, ResultCode.InvalidTeamId);
                }

                if (attribute.EntityType == typeof(User))
                {
                    CheckEntity<User>(attribute.IdPropertyName, ResultCode.InvalidUserId);
                }

                if (attribute.EntityType == typeof(Game))
                {
                    CheckEntity<Game>(attribute.IdPropertyName, ResultCode.InvalidGameId);
                }

                if (attribute.EntityType == typeof(QuizItem))
                {
                    CheckEntity<QuizItem>(attribute.IdPropertyName, ResultCode.InvalidQuizItemId);
                }
            }
        }

        private void CheckEntity<TEntity>(string entityIdPropertyName, ResultCode resultCode) where TEntity : Model, new()
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

            var entityId = (string) value;
            var entityCollection = UnitOfWork.GetCollection<TEntity>();
            var entity = entityCollection.GetAsync(entityId).Result;
            if (entity == null)
            {
                throw new DomainException(resultCode, $"Invalid {entityIdPropertyName}.", true);
            }
        }
    }
}