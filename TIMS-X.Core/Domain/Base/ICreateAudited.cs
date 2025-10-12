using System;

namespace TIMS_X.Core.Domain.Base
{
    public interface ICreateByUserAudited
    {
        int? CreatedUserId { get; set; }
    }

    public interface ICreateDateAudited
    {
        DateTime CreatedDate { get; set; }
    }
}
