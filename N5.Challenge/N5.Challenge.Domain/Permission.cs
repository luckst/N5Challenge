namespace N5.Challenge.Domain
{
    public class Permission : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public Guid PermissionTypeId { get; set; }
        public virtual PermissionType PermissionType { get; set; }
        public bool Enabled { get; set; }
    }
}
