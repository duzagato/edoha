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

        public static string SelectAllUserInformation = "SELECT id, name, phone FROM edoha.user";

        public static string SelectAllUserInformationWithTicketbook = "SELECT id, name, phone FROM edoha.user WHERE id IN (SELECT id_holder FROM lottery.ticketbook) OR id IN (SELECT id_owner FROM lottery.ticketbook)";

        public static string LotteryTicketbookNumberExists = "SELECT EXISTS (SELECT 1 FROM \"lottery\".\"ticketbook\" WHERE \"id_lottery\" = @IdLottery AND \"number\" = @Number);";

        public static string LotteryIsUnique = "SELECT COUNT(*) FROM lottery.lottery WHERE id_institution = :Id AND name = :Name";

        public static string SelectAllLotteriesByInstitution = "SELECT * FROM lottery.lottery WHERE id_institution = @IdInstitution";

        public static string TicketExists = "SELECT EXISTS (SELECT 1 FROM \"lottery\".\"ticket\" WHERE \"id_ticketbook\" = @IdTicketbook AND \"number\" = @Number);";

        public static string SelectAllTicketsByTicketbook = "SELECT * FROM \"lottery\".\"ticket\" WHERE \"id_ticketbook\" = @IdTicketbook;";

        public static string SelectTicketByIdAndTicketbook = "SELECT * FROM \"lottery\".\"ticket\" WHERE \"id\" = @Id AND \"id_ticketbook\" = @IdTicketbook;";

        public static string TicketbookConfiguration = "SELECT tb.number, l.num_tickets_ticketbook, l.num_ticketbooks, l.double_chance FROM lottery.ticketbook AS tb INNER JOIN lottery.lottery AS l ON l.id = tb.id_lottery WHERE tb.id = @IdTicketbook";

        public static string SelectTicketbookByNumber = "SELECT t.*, h.id AS Id, h.name AS Name, h.phone AS Phone, o.id AS Id, o.name AS Name, o.phone AS Phone FROM lottery.ticketbook t LEFT JOIN edoha.user h ON t.id_holder = h.id INNER JOIN edoha.user o ON t.id_owner = o.id WHERE t.id_lottery = @IdLottery AND t.number = @Number";

        public static string SelectInstitutionsByUser = "SELECT i.* FROM edoha.institution AS i INNER JOIN edoha.user_institution AS ui ON i.id = ui.id_institution WHERE ui.id_user = @IdUser";

        // Use aliases claros para evitar confusão, embora o splitOn se baseie na ordem
        public static string SelectAllTicketbooks = @"
        SELECT 
            t.*, 
            h.id, h.name, h.phone, -- Início do Holder
            o.id, o.name, o.phone  -- Início do Owner
        FROM lottery.ticketbook t 
        LEFT JOIN edoha.user h ON t.id_holder = h.id 
        INNER JOIN edoha.user o ON t.id_owner = o.id 
        WHERE t.id_lottery = @IdLottery";

        public static string SelectUsersAndInstitutions = @"
        SELECT 
            u.*, 
            i.*
        FROM edoha.user u 
        INNER JOIN edoha.user_institution ui ON ui.id_user = u.id 
        INNER JOIN edoha.institution i ON i.id = ui.id_institution 
        WHERE Nickname = @Nickname";

        public static string SelectInstitutionBySlug = @"SELECT * FROM edoha.institution WHERE slug_name = @Slug";
    }
}
