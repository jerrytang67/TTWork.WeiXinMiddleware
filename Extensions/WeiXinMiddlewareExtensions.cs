using Microsoft.AspNetCore.Builder;

namespace TTWork.WeiXinMiddleware.Extensions
{
    public static class WeiXinMiddlewareExtensions
    {
        public static void UseWeiXin(this IApplicationBuilder app, WeiXinOptions options)
        {
            app.UseMiddleware<TTWork.WeiXinMiddleware.WeiXinMiddleware>(options);
        }
    }
}