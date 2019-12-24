using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        public AuthDto Auth { get; set; }
        public string[] AllowOrigins { get; set; }
        public WeChatDto WeChat { get; set; }
    }
    public class AuthDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string ApiName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServerUrl { get; set; }
    }
    public class WeChatDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 微信回调请求地址
        /// </summary>
        public string CallbackPath { get; set; }
    }
}
