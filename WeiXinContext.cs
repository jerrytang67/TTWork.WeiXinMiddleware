using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TT.WeiXinMiddleware
{
    public class WeiXinContextBase
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; protected set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; protected set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public long CreateTime { get; protected set; }

        /// <summary>
        /// 消息类型，event
        /// </summary>
        public string MsgType { get; protected set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public long MsgId { get; protected set; }

        public string Event { get; protected set; }

        public string EventKey { get; protected set; }

        public string Content { get; protected set; }
    }


    public class WeiXinContext : WeiXinContextBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        public WeiXinContext(WeiXinMessage message, HttpContext context, List<int> tenatnList)
        {
            HttpContext = context;
            FromUserName = message.FromUserName;
            ToUserName = message.ToUserName;
            CreateTime = message.CreateTime;
            MsgId = message.MsgId;
            MsgType = message.MsgType;
            Event = message.Event;
            EventKey = message.EventKey;
            Content = message.Content;
            TenantList = tenatnList;
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpContext HttpContext { get; }

        public List<int> TenantList { get; }

    }
}