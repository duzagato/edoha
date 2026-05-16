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

            string query = StaticQueries.SelectUsersAndInstitutions;

            // Usamos um dicionário para garantir que teremos apenas UM objeto User,
            // mesmo que a query retorne múltiplas linhas (uma para cada instituição).
            var userDictionary = new Dictionary<Guid, User>();

            var result = await _connection.QueryAsync<User, Institution, User>(
                query,
                (user, institution) =>
                {
                    if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.Institutions = new List<Institution>();
                        userDictionary.Add(userEntry.Id, userEntry);
                    }

                    if (institution != null)
                    {
                        userEntry.Institutions.Add(institution);
                    }

                    return userEntry;
                },
                new { Nickname = nickname },
                splitOn: "id"
            );

            return userDictionary.Values.FirstOrDefault();
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
