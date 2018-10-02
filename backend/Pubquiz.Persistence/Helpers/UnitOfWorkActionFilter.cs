using Microsoft.AspNetCore.Mvc.Filters;

namespace Pubquiz.Persistence.Helpers
{
    public class UnitOfWorkActionFilter : ActionFilterAttribute
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkActionFilter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception==null)
            {
                _unitOfWork.Commit();    
            }            
            base.OnActionExecuted(context);
        }
    }
}