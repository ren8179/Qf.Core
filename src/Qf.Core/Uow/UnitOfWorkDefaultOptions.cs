using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Qf.Core.Uow
{
    public class UnitOfWorkDefaultOptions
    {
        public UnitOfWorkTransactionBehavior TransactionBehavior { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        public TimeSpan? Timeout { get; set; }

        internal UnitOfWorkOptions Normalize(UnitOfWorkOptions options)
        {
            if (options.IsolationLevel == null)
            {
                options.IsolationLevel = IsolationLevel;
            }

            if (options.Timeout == null)
            {
                options.Timeout = Timeout;
            }

            return options;
        }

        public bool CalculateIsTransactional(bool autoValue)
        {
            switch (TransactionBehavior)
            {
                case UnitOfWorkTransactionBehavior.Enabled:
                    return true;
                case UnitOfWorkTransactionBehavior.Disabled:
                    return false;
                case UnitOfWorkTransactionBehavior.Auto:
                    return autoValue;
                default:
                    throw new EPTException("Not implemented TransactionBehavior value: " + TransactionBehavior);
            }
        }
    }
    public enum UnitOfWorkTransactionBehavior
    {
        Auto,

        Enabled,

        Disabled
    }
}
