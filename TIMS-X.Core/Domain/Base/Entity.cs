namespace TIMS_X.Core.Domain.Base
{
    public interface IEntity
    {
        bool HasStateBeenSet { get; set; }
        int Id { get; set; }
    }

    public abstract class AuditableEntity : Entity, IAuditableEntity
    {
        public bool HasBeenAudited { get; set; }
    }

    public interface IAuditableEntity : IEntity
    {
        bool HasBeenAudited { get; set; }
    }

    public abstract class Entity : IEntity
    {
        public bool PendingDelete { get; set; }
        public bool IsNew() => Id <= 0;

        public int Id { get; set; }

        public bool HasStateBeenSet { get; set; }
    }
}