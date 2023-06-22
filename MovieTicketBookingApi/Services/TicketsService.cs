using AutoMapper;
using Core.Interfaces;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Empty;
using MovieTicketBookingApi.Protos.Tickets;
using MovieTicketBookingApi.Protos.Paging;

namespace MovieTicketBookingApi.Services;

public class TicketsService : Tickets.TicketsBase
{
    private readonly ITicketsRepository _repository;
    private readonly IMapper _mapper;

    public TicketsService(
        ITicketsRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<GetAllTicketsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context) =>
        _mapper.Map<GetAllTicketsReply>(await _repository.GetAllAsync());

    public override async Task<GetTicketByIdReply> GetById(GetTicketByIdRequest request, ServerCallContext context)
    {
        var ticket = await _repository.GetByIdAsync(new Guid(request.Id));

        if (ticket is null)
        {
            throw new NullReferenceException();
        }

        return _mapper.Map<GetTicketByIdReply>(ticket);
    }

    public override async Task<CreateTicketReply> Create(CreateTicketRequest request, ServerCallContext context)
    {
        var ticket = _repository.Create(_mapper.Map<Core.Entities.Ticket>(request));
        await _repository.SaveChangesAsync();

        return _mapper.Map<CreateTicketReply>(ticket);
    }

    public override async Task<EmptyReply> Delete(DeleteTicketRequest request, ServerCallContext context)
    {
        var ticket = await _repository.GetByIdAsync(new Guid(request.Id));

        if (ticket is null)
        {
            throw new NullReferenceException();
        }

        _repository.Delete(ticket);
        await _repository.SaveChangesAsync();

        return new EmptyReply();
    }
}
