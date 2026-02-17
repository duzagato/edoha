using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Constants
{
    public static class  StaticQueries
    {
        public static string SchemaEdoha = "edoha";

        public static string UserPermissionsExpandView = "vw_user_permission_page";

        public static string SelectUserActions = "" +
            "SELECT DISTINCT action_name, without_owner, other_owner " +
            $"FROM {SchemaEdoha}.{UserPermissionsExpandView} " +
            "WHERE id_user = @IdUser AND page_name = @PageName";

        public static string SelectUserActionByName = "" +
            "SELECT DISTINCT action_name, without_owner, other_owner " +
            $"FROM {SchemaEdoha}.{UserPermissionsExpandView} " +
            "WHERE id_user = @IdUser AND page_name = @PageName AND action_name = @ActionName";

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

        public static string SelectAllConfigurationsByTableName = "SELECT * FROM edoha.table_configuration " +
            "WHERE table_schema = @Schema AND table_name = @TableName";

        public static string SelectReturnedsTicketbooksByLottery = "SELECT * FROM lottery.ticketbook WHERE id_lottery = @IdLottery AND id_status_ticketbook IN (SELECT id FROM lottery.status_ticketbook WHERE name = @NameStatusTicketbook)";

        public static string SelectTicketbookByStatus = "SELECT * FROM lottery.ticketbook WHERE id_lottery = @IdLottery AND id_status_ticketbook = @IdStatusTicketbook";

        public static string UpdateStatusTicketbook = "UPDATE lottery.ticketbook SET id_status_ticketbook = @IdStatusTicketbook WHERE id = @IdTicketbook";

        public static string UpdateStatusTicketbookToReturned = "UPDATE lottery.ticketbook SET id_status_ticketbook = @IdStatusTicketbook, devolution_date = NOW() WHERE id = @IdTicketbook";

        public static string UpdateStatusTicketbookToWithdraw = "UPDATE lottery.ticketbook SET id_status_ticketbook = @IdStatusTicketbook, withdrawn_date = NOW() WHERE id = @IdTicketbook";
    }
}
