using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPTicketingPS.Application.Seats.Dtos;

namespace TPTicketingPS.Application.Seats
{
    public interface IGetSeats
    {
        Task<SeatMapDto> ExecuteAsync(
            int eventId,
            CancellationToken cancellationToken);
    }
}
