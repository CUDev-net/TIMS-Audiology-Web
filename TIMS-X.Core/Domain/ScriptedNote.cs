using System;
using TIMS_X.Core.Domain.Base;

namespace TIMS_X.Core.Domain
{
    public class ScriptedNote : Entity, IUpdateAudited
    {
        public bool Inactive { get; set; }
        public string Name { get; set; }
        public string Txt { get; set; }
        public bool Protected { get; set; }
        public int CategoryId { get; set; }
        public ScriptedNoteCategory Category { get; set; }
        public bool InsertedDate { get; set; }
        public bool InsertInitials { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}