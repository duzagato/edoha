using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Lottery;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ILotteryService : IService<Lottery>
    {
        public Task InsertLottery(Guid idInstitution, CreateLotteryDTO dto);

        public Task<Lottery> SelectLotteryById(Guid idInstitution, Guid id);

        public Task<IEnumerable<Lottery>> SelectAllLotteries(Guid idInstitution);

        public Task<Lottery?> GetLotteryByName(Guid idInstitution, string nameLottery);

        public Task UpdateLotteryById(Guid idInstitution, UpdateLotteryDTO dto);

        public Task DeleteLotteryById(Guid idInstitution, Guid id);
    }
}
