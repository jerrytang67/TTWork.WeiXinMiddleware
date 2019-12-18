using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TT.WeiXinMiddleware;
using static System.Int32;

namespace TTWork.WeiXinMiddleware
{
    public class WeiXinMiddleware
    {
        private RequestDelegate Next = null;
        public IConfiguration Configuration { get; }
        public WeiXinOptions Options { get; set; }
        public WeiXinMiddleware(RequestDelegate next, IConfiguration configuration, WeiXinOptions options)
        {
            Next = next;
            Configuration = configuration;
            Options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Trim().ToLower().StartsWith(Options.Path.ToLower()))
            {
                //微信服务
                if (Options.Provider == null)
                    Options.Provider = context.RequestServices.GetService(typeof(IWeiXinProvider)) as IWeiXinProvider;

                if (Options.MutilTenant)
                {
                    var tenantIds = Regex.Matches(context.Request.Path.Value.Trim().ToLower(), @"(\d+)").Select(x => Parse(x.Groups[1].Value)).ToList();

                    await Options.Provider.Run(context, Configuration, tenantIds);
                }
                else
                    await Options.Provider.Run(context, Configuration);

                return;
            }
            if (Next != null) await Next.Invoke(context);
        }
    }
}