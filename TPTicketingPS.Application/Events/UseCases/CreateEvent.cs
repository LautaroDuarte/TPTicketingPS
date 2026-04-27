using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Application.Events.Dtos;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Events.UseCases
{
    public class CreateEvent(IAppDbContext context) : ICreateEvent
    {
        public async Task<int> ExecuteAsync(CreateEventRequest request)
        {
            var ev = new Event(
                request.Name,
                request.EventDate,
                request.Venue,
                request.Description,
                request.MaxReservationsPerUser ?? int.MaxValue 
            );

            context.Events.Add(ev);
            await context.SaveChangesAsync();

            return ev.Id;
        }
    }
}
