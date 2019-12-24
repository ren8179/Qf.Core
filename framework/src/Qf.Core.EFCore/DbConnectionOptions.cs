using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore
{
    public class DbConnectionOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public DbConnectionOptions()
        {
            ConnectionStrings = new ConnectionStrings();
        }
    }
}
