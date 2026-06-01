using Amazon.Runtime.Internal.Transform;
using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
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
        public UserRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) 
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<User?> SelectUserCredentialsByNickname(string nickname)
        {
            CheckConnection();

            User? user = null;
            string query = StaticQueries.SelectUsersAndInstitutions;

            await _connection.QueryAsync<User, Institution, User>(
                query,
                (currentUser, institution) =>
                {
                    if(user is null)
                    {
                        user = currentUser!;
                    }

                    if (institution is not null)
                    {
                        user.Institutions.Add(institution);
                    }

                    return user;
                },
                new { Nickname = nickname },
                splitOn: "id"
            );

            return user;
        }

        public async Task<IEnumerable<UserInformationResponse?>> SelectUserInformation(bool withTicketbooks)
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
