using AutoMapper;
using Core.Interfaces.Repositories;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Tickets;

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
        _mapper.Map<GetAllTicketsReply>(await _repository.GetAllAsync(request.PageNumber, request.PageSize));

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
        var ticket = _mapper.Map<Core.Entities.Ticket>(request);
        await _repository.Create(ticket);

        return _mapper.Map<CreateTicketReply>(ticket);
    }

    public override async Task<EmptyReply> Delete(DeleteTicketRequest request, ServerCallContext context)
    {
        var ticket = await _repository.GetByIdAsync(new Guid(request.Id));

        if (ticket is null)
        {
            throw new NullReferenceException();
        }

        await _repository.Delete(ticket.Id);

        return new EmptyReply();
    }
}
