import { useLocation, useParams, Link } from "react-router-dom";
import { useReservation } from "../hooks/useReservation";
import { getPaymentMethod } from "../constants/paymentMethods";
import { formatCurrency, formatDateTime } from "../lib/format";
import LoadingState from "../components/LoadingState";
import ReservationItems from "../components/reservations/ReservationItems";

export default function ConfirmationPage() {
  const { reservationId } = useParams();
  const location = useLocation();

  // Si venimos de la pantalla de pago ya tenemos la reserva en el state del
  // router; useReservation la usa como valor inicial y evita un fetch extra.
  const { reservation, loading } = useReservation(reservationId, {
    initial: location.state?.reservation || null,
  });

  const method = getPaymentMethod(location.state?.paymentMethod);

  if (loading) {
    return <LoadingState />;
  }

  if (!reservation) {
    return <div className="alert alert-warning">Reserva no encontrada.</div>;
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <div className="text-center mb-4">
          <div
            className="d-inline-flex align-items-center justify-content-center rounded-circle mb-3"
            style={{ width: 80, height: 80, background: "var(--color-1)" }}
          >
            <i className="bi bi-check-lg text-white" style={{ fontSize: 48 }}></i>
          </div>
          <h2>¡Reserva confirmada!</h2>
          <p className="text-overridden-muted">
            Te enviamos un email con los detalles de tu compra.
          </p>
        </div>

        <div className="card shadow-sm mb-4">
          <div className="card-header">
            <strong>
              <i className="bi bi-receipt me-2"></i>
              Comprobante
            </strong>
          </div>
          <div className="card-body">
            <div className="row mb-3 small">
              <div className="col-6">
                <span className="text-overridden-muted">N° de reserva</span>
                <p className="mb-0 font-monospace small">{reservation.id}</p>
              </div>
              <div className="col-6 text-end">
                <span className="text-overridden-muted">Fecha</span>
                <p className="mb-0">{formatDateTime(reservation.reservedAt)}</p>
              </div>
            </div>

            <hr />

            <h6 className="mb-3">Asientos</h6>
            <ReservationItems items={reservation.items} showIcon />

            <hr />

            <div className="row">
              <div className="col-6">
                <p className="mb-1 small text-overridden-muted">Método de pago</p>
                <p className="mb-0">
                  <i className={`bi ${method.icon} me-2`}></i>
                  {method.label}
                </p>
              </div>
              <div className="col-6 text-end">
                <p className="mb-1 small text-overridden-muted">Total pagado</p>
                <p className="mb-0 fs-4 fw-bold">
                  {formatCurrency(reservation.totalAmount)}
                </p>
              </div>
            </div>
          </div>
        </div>

        <div className="d-flex gap-2 justify-content-center flex-wrap">
          <Link to="/events" className="btn btn-outline-primary">
            <i className="bi bi-house me-2"></i>
            Volver al inicio
          </Link>
          <button className="btn btn-primary" onClick={() => window.print()}>
            <i className="bi bi-printer me-2"></i>
            Imprimir comprobante
          </button>
        </div>
      </div>
    </div>
  );
}