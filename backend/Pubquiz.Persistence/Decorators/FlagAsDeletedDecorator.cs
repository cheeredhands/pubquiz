using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Pubquiz.Persistence.Decorators
{
    /// <summary>
    ///     Decorator to used flagging instead of deleting
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlagAsDeletedDecorator<T> : CollectionDecoratorBase<T> where T : Model, new()
    {
        /// <inheritdoc />
        public FlagAsDeletedDecorator(IMemoryCache memoryCache, ICollection<T> decoree) :
            base(memoryCache, decoree)
        { }

        public override async Task<T> GetAsync(Guid id)
        {
            var document = await base.GetAsync(id);
            return (document != null && document.IsDeleted == false) ? document : null;
        }
        public override IQueryable<T> AsQueryable() =>
            base.AsQueryable().Where(o => o.IsDeleted == false);
        
        public override async Task<bool> DeleteAsync(Guid id)
        {
            var document = await base.GetAsync(id);
            if (document == null) return false;
            document.IsDeleted = true;
            return await UpdateAsync(document);
        }
     
    }
}