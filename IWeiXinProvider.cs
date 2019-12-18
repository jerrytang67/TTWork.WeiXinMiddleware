using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TTWork.WeiXinMiddleware
{
    public interface IWeiXinProvider
    {
        WeiXinOptions Options { get; set; }

        Task Run(HttpContext context, IConfiguration configuration, List<int> tenantId = null);
    }
}