using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TTWork.WeiXinMiddleware
{
    public class WeiXinProvide : IWeiXinProvider
    {
        private readonly ILogger _logger;

        public WeiXinProvide(ILogger logger)
        {
            _logger = logger;
        }

        public WeiXinOptions Options { get; set; }


        /// <summary>
        /// <![CDATA[运行服务]]>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public async Task Run(HttpContext context, IConfiguration configuration, List<int> tenantIds = null)
        {
            try
            {
                #region 1、验证签名

                if (context.Request.Method.ToUpper() == "GET")
                {
                    context.Response.ContentType = "text/plain;charset=utf-8";
                    context.Response.StatusCode = 200;

                    //1、验证签名
                    if (Utils.Security.CheckSignature(context.Request.Query["nonce"],
                            context.Request.Query["timestamp"],
                            context.Request.Query["signature"],
                            configuration.GetSection("WeiXinOAuth")["Token"]))
                    {
                        await context.Response.WriteAsync(context.Request.Query["echostr"]);
                        return;
                    }

                    await context.Response.WriteAsync("无效签名！");
                    return;
                }

                #endregion 1、验证签名

                #region 2、接收微信消息

                await OnRecieve(context, tenantIds); //接收消息

                #endregion 2、接收微信消息
            }
            catch (Exception ex)
            {
                _logger.LogError("运行服务 {@WeiXinProvideError}", ex);
                await context.Response.WriteAsync(ex.Message);
            }
        }

        #region 虚方法

        /// <summary>
        /// <![CDATA[序列化]]>
        /// </summary>
        private static readonly XmlSerializer _XmlSerializer = new XmlSerializer(typeof(WeiXinMessage));

        /// <summary>
        /// <![CDATA[虚方法，接收消息后处理]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnRecieve(HttpContext context, List<int> tenantIds)
        {
            if (Options.OnRecieveAsync != null) return Options.OnRecieveAsync(context);
            string recieveBody = null; //接收消息

            using (System.IO.StreamReader streamReader = new System.IO.StreamReader(context.Request.Body))
            {
                recieveBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
                _logger.LogInformation("接收内容：{@RecieveBody}", recieveBody);
                recieveBody = Utils.Security.ClearXmlHeader(recieveBody);
            }

            //反序列化
            var recieve = _XmlSerializer.Deserialize(recieveBody) as WeiXinMessage;

            LogTask(tenantIds, recieve, recieveBody);

            //事件消息
            if (recieve.MsgType == Constants.MSG_TYPE.EVENT)
            {
                var weiXinContext = new WeiXinContext(recieve, context, tenantIds);
                var actionName = recieve.Event.ToLower();
                actionName = actionName.First().ToString().ToUpper() + actionName.Substring(1);
                var action = this.GetType().GetMethod($"On{actionName}");
                if (action != null) return (Task)action.Invoke(this, new object[] { weiXinContext });
            }
            //被动接收消息
            else
            {
                return OnRecieveMessage(new WeiXinContext(recieve, context, tenantIds));
            }

            return Task.Delay(0);
        }


        public virtual void LogTask(List<int> tenantIds, WeiXinMessage recieve, string body)
        {
            //NotImplementedException do nothing!!
        }

        /// <summary>
        /// <![CDATA[被动接收消息]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnRecieveMessage(WeiXinContext context)
        {
            if (Options.OnRecieveMessageAsync != null)
                return Options.OnRecieveMessageAsync(context);
            return Task.Delay(0);
        }


        /// <summary>
        /// <![CDATA[扫描事件]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnScan(WeiXinContext context)
        {
            if (Options.OnScanAsync != null) return Options.OnScanAsync(context);
            return Task.Delay(0);
        }

        /// <summary>
        /// <![CDATA[关注事件]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnSubscribe(WeiXinContext context)
        {
            if (Options.OnSubscribeAsync != null) return Options.OnSubscribeAsync(context);
            return Task.Delay(0);
        }

        /// <summary>
        /// <![CDATA[取消关注]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnUnsubscribe(WeiXinContext context)
        {
            if (Options.OnUnsubscribeAsync != null) return Options.OnUnsubscribeAsync(context);
            return Task.Delay(0);
        }

        /// <summary>
        ///  <![CDATA[菜单点击]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnClick(WeiXinContext context)
        {
            if (Options.OnClickAsync != null) return Options.OnClickAsync(context);
            return Task.Delay(0);
        }

        /// <summary>
        /// <![CDATA[点击菜单链接]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnView(WeiXinContext context)
        {
            if (Options.OnViewAsync != null) return Options.OnViewAsync(context);
            return Task.Delay(0);
        }

        /// <summary>
        /// <![CDATA[上报地理位置]]>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task OnLocation(WeiXinContext context)
        {
            if (Options.OnLocationAsync != null) return Options.OnLocationAsync(context);
            return Task.Delay(0);
        }

        #endregion
    }
}