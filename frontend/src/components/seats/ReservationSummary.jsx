import Spinner from "../Spinner";
import { formatCurrency } from "../../lib/format";

/**
 * Resumen lateral de la selección + botón para continuar la reserva.
 * Presentacional: el cálculo del total y el submit vienen por props.
 */
export default function ReservationSummary({
  selectedSeats,
  totalAmount,
  onConfirm,
  submitting,
  disabled,
}) {
  const hasSeats = selectedSeats.length > 0;

  return (
    <div className="card shadow-sm summary-card">
      <div className="card-header bg-primary text-white">
        <i className="bi bi-cart3 me-2"></i> Resumen
      </div>
      <div className="card-body">
        {!hasSeats ? (
          <p className="text-overridden-muted small mb-0">
            No hay asientos seleccionados.
          </p>
        ) : (
          <>
            <ul className="list-unstyled small mb-3">
              {selectedSeats.map((seat) => (
                <li
                  key={seat.id}
                  className="d-flex justify-content-between mb-1"
                >
                  <span>
                    {seat.sectorName} {seat.rowIdentifier}
                    {seat.seatNumber}
                  </span>
                  <span className="text-overridden-muted">
                    {formatCurrency(seat.price)}
                  </span>
                </li>
              ))}
            </ul>
            <div className="d-flex justify-content-between fw-bold border-top pt-2">
              <span>Total:</span>
              <span>{formatCurrency(totalAmount)}</span>
            </div>
          </>
        )}

        <button
          className="btn btn-primary w-100 mt-3"
          disabled={disabled || !hasSeats || submitting}
          onClick={onConfirm}
        >
          {submitting ? (
            <>
              <Spinner small className="me-2" />
              Reservando...
            </>
          ) : (
            "Continuar reserva"
          )}
        </button>
      </div>
    </div>
  );
}