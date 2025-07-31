using Manager.Domain.Logging;
using Manager.Domain.Mappers;
using Manager.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Manager.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketMapper _mapper;
        private readonly ITicketRepository _repo;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository tokenClienteRepository, ITicketMapper tokenClienteMapper, ILogger<TicketService> logger)
        {
            _repo = tokenClienteRepository;
            _mapper = tokenClienteMapper;
            _logger = logger;
        }

        public async Task<TicketResponse> GetTicketAsync(GetTicketRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.clienteId == null)
                throw new ArgumentNullException(nameof(request.clienteId));
            if (string.IsNullOrWhiteSpace(request.codProceso))
                throw new ArgumentNullException(nameof(request.codProceso));
            if (string.IsNullOrWhiteSpace(request.perTributario))
                throw new ArgumentNullException(nameof(request.perTributario));

            var entity = await _repo.GetAsync(
                request.clienteId,
                request.codProceso,
                request.perTributario);

            _logger.LogInformation(Logging.Events.GetById, Messages.TargetEntityChanged_id, entity?.Id);

            return _mapper.Map(entity);
        }

        public async Task<TicketResponse> AddTicketAsync(AddTicketRequest request)
        {
            var tokenCliente = _mapper.Map(request);

            var result = await _repo.AddAsync(tokenCliente);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map(result);
        }

        public async Task<TicketResponse> EditTicketAsync(EditTicketRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.Id);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.Id} is not present");

            var entity = _mapper.Map(request);
            var result = await _repo.UpdateAsync(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map(result);
        }

        //public async Task<TicketResponse> DeleteTicketAsync(DeleteTicketRequest request)
        //{
        //    if (request?.Id == null) throw new ArgumentNullException();

        //    var result = await _repo.GetAsync(request.Id);
        //    result.IsInactive = true;

        //    _repo.Update(result);
        //    await _repo.UnitOfWork.SaveChangesAsync();

        //    return _mapper.Map(result);
        //}
    }
}