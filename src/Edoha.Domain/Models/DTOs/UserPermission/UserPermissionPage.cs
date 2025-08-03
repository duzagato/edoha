using PageAction = Edoha.Domain.Entities.Action;

namespace Edoha.Domain.Models.DTOs.UserPermission
{
    public class UserPermissionPage
    {
        public string PageName { get; set; }
        public IEnumerable<PageAction> Actions { get; set; }

        public UserPermissionPage(string pageName, IEnumerable<PageAction> actions)
        {
            PageName = pageName;
            Actions = actions;
        }
    }
}
