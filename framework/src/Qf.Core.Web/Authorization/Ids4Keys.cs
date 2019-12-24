using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qf.Core.Web.Authorization
{
    /// <summary>
    /// 键
    /// </summary>
    public class Ids4Keys
    {
        /// <summary>
        /// Gets or sets 键
        /// </summary>
        public JsonWebKey[] Keys { get; set; }
    }
}
