using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.DependencyInjection
{
    public class ObjectAccessor<T> : IObjectAccessor<T>
    {
        public T Value { get; set; }

        public ObjectAccessor()
        {

        }

        public ObjectAccessor([CanBeNull] T obj)
        {
            Value = obj;
        }
    }
}
