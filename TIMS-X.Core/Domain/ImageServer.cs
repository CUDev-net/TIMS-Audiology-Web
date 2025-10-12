using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Domain
{
    public class ImageServer
    {
        public Guid Id { get; set; }
        public string DatabaseName { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveStorage { get; set; }
        public int UpdatedUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
