using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ResultMsg
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public dynamic Result { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ResultMsg<T>
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public T Result { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
    }
}
