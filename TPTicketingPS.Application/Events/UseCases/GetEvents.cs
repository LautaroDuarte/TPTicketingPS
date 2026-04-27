using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events.UseCases;

public class GetEvents(IAppDbContext context) : IGetEvents
{
    public async Task<List<EventSummaryDto>> ExecuteAsync(EventQueryParameters parameters)
    {
        var query = context.Events.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.Status))
        {
            query = query.Where(e => e.Status.ToString() == parameters.Status);
        }

        var events = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return events.Select(e => new EventSummaryDto(
            e.Id,
            e.Name,
            e.EventDate,
            e.Venue,
            e.Status.ToString()
        )).ToList();
    }
}