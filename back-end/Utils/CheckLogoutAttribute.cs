using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Utils
{
    public class CheckLogoutAttribute : TypeFilterAttribute
    {
        public CheckLogoutAttribute() : base(typeof(CheckLogoutFilter))
        {
        }
    }

    public class CheckLogoutFilter : IAsyncActionFilter
    {
        private IContext _Context;
        public CheckLogoutFilter(IContext context)
        {
            _Context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // execute any code before the action executes
            IActionResult result = AuthorizationCheck(context);

            if (result != null)
                context.Result = result;
            else
                await next();

            // execute any code after the action executes
        }

        private IActionResult AuthorizationCheck(ActionExecutingContext context)
        {
            IActionResult result = null;
            ControllerBase controller = context.Controller as ControllerBase;

            if (controller?.User != null)
            {
                if (controller.User.Claims.Count() > 0)
                {
                    if (_Context.IsTokenBlackList(_Context.GetCurrentToken(controller.Request)))
                    {
                        result = new UnauthorizedResult();
                    }
                }
            }

            return result;
        }
    }
}
