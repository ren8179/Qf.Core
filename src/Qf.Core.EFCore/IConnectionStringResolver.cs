using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.EFCore
{
    public interface IConnectionStringResolver
    {
        string Resolve(string connectionStringName = null);
    }
}
