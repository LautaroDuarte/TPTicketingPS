using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events;

public interface IGetEvents
{
    Task<List<EventSummaryDto>> ExecuteAsync(
        EventQueryParameters parameters,
        CancellationToken cancellationToken = default);
}
