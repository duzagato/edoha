using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Lottery;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class LotteryService : Service<Lottery>, ILotteryService
    {
        ILotteryRepository _lotteryRepository;
        IInstitutionRepository _institutionRepository;

        public LotteryService(ILotteryRepository repository,
            IInstitutionRepository institutionRepository,
            IRequestValidationContext requestValidationContext)
            : base(repository, requestValidationContext) 
        {
            _lotteryRepository = repository;
            _institutionRepository = institutionRepository;
        }

        public async Task InsertLottery(Guid idInstitution, CreateLotteryDTO dto)
        {
            await _institutionRepository.IdExists(idInstitution);

            bool lotteryIsUnique = await _lotteryRepository.LotteryIsUnique(idInstitution, dto.Name);

            if (!lotteryIsUnique)
            {
                await _requestValidationContext.AddError("name", "Já existe uma rifa com esse nome");
            }

            dto.IdInstitution = idInstitution;
            await Insert(dto);
        }

        public async Task<Lottery> SelectLotteryById(Guid idInstitution, Guid id)
        {
            await _institutionRepository.IdExists(idInstitution);
            var lottery = await _repository.SelectById(id);
            if (lottery.IdInstitution != idInstitution)
                throw new KeyNotFoundException("Entidade não encontrada");
            return lottery;
        }

        public async Task<IEnumerable<Lottery>> SelectAllLotteries(Guid idInstitution)
        {
            await _institutionRepository.IdExists(idInstitution);
            return await _lotteryRepository.SelectAllByInstitution(idInstitution);
        }

        public async Task<Lottery?> GetLotteryByName(Guid idInstitution, string nameLottery)
        {
            await _institutionRepository.IdExists(idInstitution);
            return await _lotteryRepository.SelectLotteryByName(idInstitution, nameLottery);
        }

        public async Task UpdateLotteryById(Guid idInstitution, UpdateLotteryDTO dto)
        {
            await _institutionRepository.IdExists(idInstitution);
            var lottery = await _repository.SelectById(dto.Id);
            if (lottery.IdInstitution != idInstitution)
                throw new KeyNotFoundException("Entidade não encontrada");
            await Update(dto);
        }

        public async Task DeleteLotteryById(Guid idInstitution, Guid id)
        {
            await _institutionRepository.IdExists(idInstitution);
            var lottery = await _repository.SelectById(id);
            if (lottery.IdInstitution != idInstitution)
                throw new KeyNotFoundException("Entidade não encontrada");
            await DeleteById(id);
        }
    }
}
