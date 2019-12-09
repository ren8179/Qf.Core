using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public class ServiceExposingActionList : List<Action<IOnServiceExposingContext>>
    {

    }
}
