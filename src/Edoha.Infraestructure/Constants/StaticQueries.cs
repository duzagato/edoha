using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Constants
{
    public static class StaticQueries
    {
        public static string SchemaEdoha = "edoha";

        public static string UserPermissionsExpandView = "vw_user_permission_page";

        public static string SelectUserActions = "" +
            "SELECT DISTINCT action_name, without_owner, other_owner " +
            $"FROM {SchemaEdoha}.{UserPermissionsExpandView} " +
            "WHERE id_user = @IdUser AND page_name = @PageName";

        public static string SelectPagesPermissionsByIdUser = "SELECT DISTINCT page_name" +
            $"FROM {SchemaEdoha}.{UserPermissionsExpandView} " +
            "WHERE id_user = @IdUser";

        public static string GetUserPermissionExpandByIdUser = "SELECT DISTINCT " +
            "page_name, action_name, without_owner, other_owner " +
            $"FROM {SchemaEdoha}.{UserPermissionsExpandView} " +
            "WHERE id_user = @IdUser";

        public static string SelectUserCredentialsByNickname = "SELECT id, nickname, password " +
            $"FROM {SchemaEdoha}.user " +
            "WHERE nickname = @Nickname";
    }
}
