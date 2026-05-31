using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Application.Common.Models;

namespace TPTicketingPS.Application.Events;

public class GetEvents(IEventRepository eventRepository) : IGetEvents
{
    public async Task<PagedResult<EventSummaryDto>> ExecuteAsync(
        EventQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var page = parameters.Page < 1 ? 1 : parameters.Page;
        var pagesize = parameters.PageSize is < 1 or > 100 ? 10 : parameters.PageSize;

        var (items, totalCount) = await eventRepository.GetPagedAsync(
            page,
            pagesize,
            parameters.Status,
            cancellationToken);

        var summaries = items
            .Select(e => new EventSummaryDto(
                Id: e.Id,
                Name: e.Name,
                EventDate: e.EventDate,
                Venue: e.Venue,
                Status: e.Status))
           .ToList();

        return new PagedResult<EventSummaryDto>(
            Items: summaries,
            Page: page,
            PageSize: pagesize,
            TotalCount: totalCount);
    }
}