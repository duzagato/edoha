using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Domain.Models.Responses.User;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

        public async Task<User?> SelectUserCredentialsByNickname(string nickname)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string query = StaticQueries.SelectUserCredentialsByNickname;

            var user = await _connection.QueryFirstOrDefaultAsync<User>(query, new { Nickname = nickname });

            return user ?? null;
        }

        public async Task<IEnumerable<UserInformationResponse>> SelectUserInformation(bool withTicketbooks)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string sql = String.Empty;

            if (withTicketbooks)
            {
                sql = StaticQueries.SelectAllUserInformationWithTicketbook;
            }
            else
            {
                sql = StaticQueries.SelectAllUserInformation;
            }

            var users = await _connection.QueryAsync<UserInformationResponse>(sql);

            return users;
        }
    }
}
