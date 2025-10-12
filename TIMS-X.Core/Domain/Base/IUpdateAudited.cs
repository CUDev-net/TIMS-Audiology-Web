using System;

namespace TIMS_X.Core.Domain.Base
{
    public interface IUpdateAudited : IUpdateDateAudited
    {
        int? UpdatedUserId { get; set; }
    }

    public interface IUpdateDateAudited
    {
        DateTime UpdatedDate { get; set; }
    }
}
