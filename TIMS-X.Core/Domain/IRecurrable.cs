using System;

namespace TIMS_X.Core.Domain
{
    public interface IRecurrable
    {
        DateTime EndsAt { get; }

        DateTime StartsAt { get; }
    }
}