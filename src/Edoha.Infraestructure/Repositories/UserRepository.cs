using Amazon.Runtime.Internal.Transform;
using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Domain.Models.Responses.User;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _repositoryLogger;
        
        public UserRepository(IDbConnectionFactory connectionFactory, ILogger<UserRepository> logger) 
            : base(connectionFactory, logger) 
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _repositoryLogger = logger;
        }

        public async Task<User?> SelectUserCredentialsByNickname(string nickname)
        {
            _repositoryLogger.LogInformation("Iniciando método SelectUserCredentialsByNickname (UserRepository)");
            _repositoryLogger.LogInformation("Parâmetros recebidos - nickname: {Nickname}", nickname);
            
            CheckConnection();

            User? user = null;
            string query = StaticQueries.SelectUsersAndInstitutions;

            _repositoryLogger.LogInformation("Executando query para selecionar usuário e instituições por nickname");
            await _connection.QueryAsync<User, Institution, User>(
                query,
                (currentUser, institution) =>
                {
                    if(user is null)
                    {
                        user = currentUser!;
                        _repositoryLogger.LogInformation("Usuário encontrado com nickname {Nickname}", nickname);
                    }

                    if (institution is not null)
                    {
                        user.Institutions.Add(institution);
                        _repositoryLogger.LogInformation("Adicionando instituição {InstitutionId} ao usuário", institution.Id);
                    }

                    return user;
                },
                new { Nickname = nickname },
                splitOn: "id"
            );

            if (user == null)
            {
                _repositoryLogger.LogInformation("Usuário com nickname {Nickname} não encontrado. Retornando null", nickname);
            }
            else
            {
                _repositoryLogger.LogInformation("Usuário com nickname {Nickname} encontrado com {Count} instituições. Retornando resultado", nickname, user.Institutions.Count);
            }

            return user;
        }

        public async Task<IEnumerable<UserInformationResponse?>> SelectUserInformation(bool withTicketbooks)
        {
            _repositoryLogger.LogInformation("Iniciando método SelectUserInformation (UserRepository)");
            _repositoryLogger.LogInformation("Parâmetros recebidos - withTicketbooks: {WithTicketbooks}", withTicketbooks);
            
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string sql = String.Empty;

            if (withTicketbooks)
            {
                _repositoryLogger.LogInformation("Selecionando usuários com ticketbooks");
                sql = StaticQueries.SelectAllUserInformationWithTicketbook;
            }
            else
            {
                _repositoryLogger.LogInformation("Selecionando usuários sem ticketbooks");
                sql = StaticQueries.SelectAllUserInformation;
            }

            _repositoryLogger.LogInformation("Executando query para selecionar informações de usuários");
            var users = await _connection.QueryAsync<UserInformationResponse>(sql);

            _repositoryLogger.LogInformation("Método SelectUserInformation finalizado. Retornando {Count} usuários", users.Count());
            return users;
        }
    }
}
