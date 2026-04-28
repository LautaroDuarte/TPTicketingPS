using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events.UseCases
{
    public interface ICreateEvent
    {
        Task<int> ExecuteAsync(CreateEventRequest request, CancellationToken cancellationToken);
    }
}
