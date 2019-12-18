using Microsoft.Extensions.DependencyInjection;

namespace TTWork.WeiXinMiddleware.Extensions
{
    public static class WeiXinServiceCollectionExtension
    {
        public static void AddWeiXinService(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IWeiXinProvider), typeof(WeiXinProvide));
        }
    }
}