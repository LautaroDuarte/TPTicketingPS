import { Link } from "react-router-dom";
import StatusBadge from "./StatusBadge";
import { formatCurrency, formatDateTime } from "../../lib/format";
import { timeRemainingLabel, isExpired } from "../../lib/time";

const DATE_OPTIONS = {
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
};

/** Tarjeta de una reserva en el listado "Mis reservas". */
export default function ReservationCard({ reservation }) {
  const isPending = reservation.status === "Pending";
  const expiredByTime = isPending && isExpired(reservation.expiresAt);
  const items = reservation.items || [];

  return (
    <div className="card">
      <div className="card-body">
        <div className="row align-items-center">
          {/* Info principal */}
          <div className="col-md-4">
            <div className="d-flex align-items-center gap-2 mb-1">
              <StatusBadge status={reservation.status} />
              {isPending && !expiredByTime && (
                <span className="badge bg-info text-dark">
                  <i className="bi bi-stopwatch me-1"></i>
                  {timeRemainingLabel(reservation.expiresAt)}
                </span>
              )}
            </div>
            <small className="text-overridden-muted d-block">
              Reservada el {formatDateTime(reservation.reservedAt, DATE_OPTIONS)}
            </small>
            <small className="text-overridden-muted d-block">
              ID: <code className="small">{reservation.id.slice(0, 8)}...</code>
            </small>
          </div>

          {/* Asientos */}
          <div className="col-md-4">
            <small className="text-overridden-muted d-block mb-1">Asientos:</small>
            <div className="d-flex flex-wrap gap-1">
              {items.map((item) => (
                <span
                  key={item.id || item.seatId}
                  className="badge bg-dark"
                  style={{ fontSize: 11 }}
                >
                  {item.sectorName || "?"} - {item.rowIdentifier}
                  {item.seatNumber}
                </span>
              ))}
            </div>
          </div>

          {/* Total y acciones */}
          <div className="col-md-4 text-md-end mt-2 mt-md-0">
            <div className="fs-5 fw-bold mb-2">
              {formatCurrency(reservation.totalAmount)}
            </div>
            {isPending && !expiredByTime && (
              <Link
                className="btn btn-primary btn-sm"
                to={`/reservations/${reservation.id}/payment`}
              >
                <i className="bi bi-credit-card me-1"></i>
                Pagar ahora
              </Link>
            )}
            {reservation.status === "Paid" && (
              <Link
                className="btn btn-outline-success btn-sm"
                to={`/reservations/${reservation.id}/confirmation`}
              >
                <i className="bi bi-receipt me-1"></i>
                Ver comprobante
              </Link>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}