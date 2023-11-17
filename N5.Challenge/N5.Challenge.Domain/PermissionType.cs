namespace N5.Challenge.Domain
{
    public class PermissionType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; }

    }
}
