import { MdEventSeat } from "react-icons/md";
import { formatCurrency } from "../../lib/format";

/**
 * Render del plano de asientos a partir de los datos ya agrupados por
 * sector/fila (`groupSeatsBySector`). Es presentacional: recibe el estado de
 * selección y los handlers, no decide nada de negocio.
 *
 * Props:
 *  - sectors: sectores con `groupedSeats`
 *  - isSelected(seatId): bool
 *  - onToggle(seat, price, sectorName)
 *  - disabled: deshabilita los botones (ej. mientras se envía la reserva)
 */
export default function SeatMap({ sectors, isSelected, onToggle, disabled }) {
  return (
    <>
      {sectors.map((sector) => (
        <div key={sector.sectorId} className="card mb-4 shadow-sm">
          <div className="card-header d-flex justify-content-between align-items-center">
            <h5 className="mb-0">{sector.sectorName}</h5>
            <span className="badge bg-primary fs-6">
              {formatCurrency(sector.price)}
            </span>
          </div>

          <div className="card-body seat-map-container">
            {Object.keys(sector.groupedSeats)
              .sort()
              .map((row) => (
                <div key={row} className="row-seats">
                  <span className="row-label">{row}</span>
                  {sector.groupedSeats[row].map((seat) => {
                    const selected = isSelected(seat.id);
                    const isAvailable = seat.status === "Available";
                    return (
                      <button
                        key={seat.id}
                        disabled={!isAvailable || disabled}
                        onClick={() =>
                          onToggle(seat, sector.price, sector.sectorName)
                        }
                        className={`seat ${selected ? "selected" : ""} ${seat.status.toLowerCase()}`}
                        title={`${seat.rowIdentifier}${seat.seatNumber} — ${seat.status}`}
                      >
                        <MdEventSeat size={14} />
                        <span className="seat-label">
                          {seat.rowIdentifier}
                          {seat.seatNumber}
                        </span>
                      </button>
                    );
                  })}
                </div>
              ))}
          </div>
        </div>
      ))}
    </>
  );
}