using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Domain.Entities
{
    public class ReservationItem
    {
        public Guid Id { get; private set; }
        public Guid ReservationId { get; private set; }
        public Reservation Reservation { get; private set; }

        public Guid SeatId { get; private set; }
        public Seat Seat { get; private set; }

        public decimal UnitPrice { get; private set; }

        private ReservationItem() { } // EF Core

        public ReservationItem(Guid reservationId, Guid seatId, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            ReservationId = reservationId;
            SeatId = seatId;
            UnitPrice = unitPrice;
        }
    }
}
