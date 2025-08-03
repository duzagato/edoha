namespace Edoha.Domain.Models.DTOs.UserPermission
{
    public class UserPermissionExpand
    {
        public string PageName { get; set; }
        public string ActionName {  get; set; }
        public bool WithoutOwner { get; set; }
        public bool OtherOwner { get; set; }
    }
}
