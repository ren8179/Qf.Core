using JetBrains.Annotations;
using Qf.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qf.Core.Uow
{
    public static class UnitOfWorkExtensions
    {
        public static bool IsReservedFor([NotNull] this IUnitOfWork unitOfWork, string reservationName)
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));

            return unitOfWork.IsReserved && unitOfWork.ReservationName == reservationName;
        }
    }
}
