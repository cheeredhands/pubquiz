using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Pubquiz.Persistence.Extensions;
using Pubquiz.Persistence.Helpers;

namespace Pubquiz.Persistence.Decorators
{
    /// <summary>
    ///     Used to fill default values like, created, createdby, modified, modifiedby, id etc..
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FillDefaultValueDecorator<T> : CollectionDecoratorBase<T> where T : Model, new()
    {
        private readonly string _actorId;

        /// <inheritdoc />
        public FillDefaultValueDecorator(IMemoryCache memoryCache, ICollection<T> decoree, string actorId)
            : base(memoryCache, decoree)
        {
            _actorId = actorId;
        }

        /// <inheritdoc />
        public override async Task<T> AddAsync(T document)
        {
            var userId = document.CreatedByUserId == Guid.Empty.ToShortGuidString()
                ? _actorId
                : document.CreatedByUserId;
            if (document.Id == Guid.Empty.ToShortGuidString()) document.Id = Guid.NewGuid().ToShortGuidString();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                document.CreatedByUserId =
                    !OverrideDefaultValues.FillDefaulValues && document.CreatedByUserId != Guid.Empty.ToShortGuidString()
                        ? document.CreatedByUserId
                        : userId;
                document.LastModifiedByUserId =
                    !OverrideDefaultValues.FillDefaulValues && document.LastModifiedByUserId != Guid.Empty.ToShortGuidString()
                        ? document.LastModifiedByUserId
                        : userId;
            }
            var dateNow = !OverrideDefaultValues.FillDefaulValues && document.Created != default(DateTime)
                ? document.Created
                : DateTime.UtcNow;
            document.Created = dateNow;
            document.LastModified = dateNow;
            return await base.AddAsync(document);
        }


        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(T document)
        {
            var userId = _actorId;

            if (!string.IsNullOrWhiteSpace(userId))
                document.LastModifiedByUserId =
                    !OverrideDefaultValues.FillDefaulValues && document.LastModifiedByUserId != Guid.Empty.ToShortGuidString()
                        ? document.LastModifiedByUserId
                        : userId;

            var dateNow = !OverrideDefaultValues.FillDefaulValues && document.LastModified != default(DateTime)
                ? document.LastModified
                : DateTime.UtcNow;
            document.LastModified = dateNow;
            return await base.UpdateAsync(document);
        }
    }
}