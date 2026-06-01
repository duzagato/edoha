using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Lottery;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public class LotteryService : Service<Lottery>, ILotteryService
    {
        ILotteryRepository _lotteryRepository;
        IInstitutionRepository _institutionRepository;
        private readonly ILogger<LotteryService> _logger;

        public LotteryService(ILotteryRepository repository,
            IInstitutionRepository institutionRepository,
            IRequestValidationContext requestValidationContext,
            ILogger<LotteryService> logger)
            : base(repository, requestValidationContext)
        {
            _lotteryRepository = repository;
            _institutionRepository = institutionRepository;
            _logger = logger;
        }

        public async Task InsertLottery(Guid idInstitution, CreateLotteryDTO dto)
        {
            _logger.LogInformation("[LotteryService.InsertLottery] Iniciado. Params: idInstitution={idInstitution}, dto={@dto}", idInstitution, dto);

            _logger.LogInformation("[LotteryService.InsertLottery] Verificando existência da instituição {idInstitution}.", idInstitution);
            await _institutionRepository.IdExists(idInstitution);

            _logger.LogInformation("[LotteryService.InsertLottery] Verificando unicidade do nome '{name}' para a instituição {idInstitution}.", dto.Name, idInstitution);
            bool lotteryIsUnique = await _lotteryRepository.LotteryIsUnique(idInstitution, dto.Name);

            if (!lotteryIsUnique)
            {
                _logger.LogWarning("[LotteryService.InsertLottery] Nome '{name}' já existe na instituição {idInstitution}. Adicionando erro de validação.", dto.Name, idInstitution);
                await _requestValidationContext.AddError("name", "Já existe uma rifa com esse nome");
            }

            dto.IdInstitution = idInstitution;

            _logger.LogInformation("[LotteryService.InsertLottery] Inserindo rifa no repositório. DTO={@dto}", dto);
            await Insert(dto);
            _logger.LogInformation("[LotteryService.InsertLottery] Rifa inserida com sucesso.");
        }

        public async Task<Lottery> SelectLotteryById(Guid idInstitution, Guid id)
        {
            _logger.LogInformation("[LotteryService.SelectLotteryById] Iniciado. Params: idInstitution={idInstitution}, id={id}", idInstitution, id);

            _logger.LogInformation("[LotteryService.SelectLotteryById] Verificando existência da instituição {idInstitution}.", idInstitution);
            await _institutionRepository.IdExists(idInstitution);

            _logger.LogInformation("[LotteryService.SelectLotteryById] Buscando rifa {id} no repositório.", id);
            var lottery = await _repository.SelectById(id);
            _logger.LogInformation("[LotteryService.SelectLotteryById] Rifa encontrada: {@lottery}", lottery);

            if (lottery.IdInstitution != idInstitution)
            {
                _logger.LogWarning("[LotteryService.SelectLotteryById] Rifa {id} não pertence à instituição {idInstitution}. Lançando exceção.", id, idInstitution);
                throw new KeyNotFoundException("Entidade não encontrada");
            }

            _logger.LogInformation("[LotteryService.SelectLotteryById] Retornando rifa {id}.", id);
            return lottery;
        }

        public async Task<IEnumerable<Lottery>> SelectAllLotteries(Guid idInstitution)
        {
            _logger.LogInformation("[LotteryService.SelectAllLotteries] Iniciado. Params: idInstitution={idInstitution}", idInstitution);

            _logger.LogInformation("[LotteryService.SelectAllLotteries] Verificando existência da instituição {idInstitution}.", idInstitution);
            await _institutionRepository.IdExists(idInstitution);

            _logger.LogInformation("[LotteryService.SelectAllLotteries] Buscando todas as rifas da instituição {idInstitution}.", idInstitution);
            var lotteries = await _lotteryRepository.SelectAllByInstitution(idInstitution);

            _logger.LogInformation("[LotteryService.SelectAllLotteries] Retornando {count} rifa(s) para a instituição {idInstitution}.", lotteries.Count(), idInstitution);
            return lotteries;
        }

        public async Task UpdateLotteryById(Guid idInstitution, UpdateLotteryDTO dto)
        {
            _logger.LogInformation("[LotteryService.UpdateLotteryById] Iniciado. Params: idInstitution={idInstitution}, dto={@dto}", idInstitution, dto);

            _logger.LogInformation("[LotteryService.UpdateLotteryById] Verificando existência da instituição {idInstitution}.", idInstitution);
            await _institutionRepository.IdExists(idInstitution);

            _logger.LogInformation("[LotteryService.UpdateLotteryById] Buscando rifa {id} para validar posse.", dto.Id);
            var lottery = await _repository.SelectById(dto.Id);

            if (lottery.IdInstitution != idInstitution)
            {
                _logger.LogWarning("[LotteryService.UpdateLotteryById] Rifa {id} não pertence à instituição {idInstitution}. Lançando exceção.", dto.Id, idInstitution);
                throw new KeyNotFoundException("Entidade não encontrada");
            }

            _logger.LogInformation("[LotteryService.UpdateLotteryById] Atualizando rifa {id}. DTO={@dto}", dto.Id, dto);
            await Update(dto);
            _logger.LogInformation("[LotteryService.UpdateLotteryById] Rifa {id} atualizada com sucesso.", dto.Id);
        }

        public async Task DeleteLotteryById(Guid idInstitution, Guid id)
        {
            _logger.LogInformation("[LotteryService.DeleteLotteryById] Iniciado. Params: idInstitution={idInstitution}, id={id}", idInstitution, id);

            _logger.LogInformation("[LotteryService.DeleteLotteryById] Verificando existência da instituição {idInstitution}.", idInstitution);
            await _institutionRepository.IdExists(idInstitution);

            _logger.LogInformation("[LotteryService.DeleteLotteryById] Buscando rifa {id} para validar posse.", id);
            var lottery = await _repository.SelectById(id);

            if (lottery.IdInstitution != idInstitution)
            {
                _logger.LogWarning("[LotteryService.DeleteLotteryById] Rifa {id} não pertence à instituição {idInstitution}. Lançando exceção.", id, idInstitution);
                throw new KeyNotFoundException("Entidade não encontrada");
            }

            _logger.LogInformation("[LotteryService.DeleteLotteryById] Deletando rifa {id}.", id);
            await DeleteById(id);
            _logger.LogInformation("[LotteryService.DeleteLotteryById] Rifa {id} deletada com sucesso.", id);
        }
    }
}
