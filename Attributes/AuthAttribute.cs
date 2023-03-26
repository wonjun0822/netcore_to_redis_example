using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace redis_example_net.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        // 인가 Filter 작성 시 받아올 role 정보
        public string role { get; set; } = string.Empty;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // session 에 id로 들어간 값이 없을 시 401 Return
            if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("id")))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401
                };

                return;
            }

            // 설정한 role 정보가 session 의 role 정보와 다르면 403 return
            else if (context.HttpContext.Session.GetString("role") != role)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403
                };

                return;
            }

            await next();
        }
    }
}