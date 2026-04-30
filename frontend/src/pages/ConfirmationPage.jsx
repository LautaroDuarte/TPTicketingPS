import { useLocation, useNavigate, useParams, Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../api/client";

export default function ConfirmationPage() {
  const { reservationId } = useParams();
  const navigate = useNavigate();
  const location = useLocation();
  const [reservation, setReservation] = useState(location.state?.reservation || null);
  const [loading, setLoading] = useState(!location.state?.reservation);

  const paymentMethod = location.state?.paymentMethod || "creditCard";

  // Si entraron directo a la URL sin pasar por payment, fetcheamos
  useEffect(() => {
    if (!reservation) {
      api.get(`/api/v1/reservations/${reservationId}`)
        .then(setReservation)
        .finally(() => setLoading(false));
    }
  }, [reservationId, reservation]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary"></div>
      </div>
    );
  }

  if (!reservation) {
    return <div className="alert alert-warning">Reserva no encontrada.</div>;
  }

  const methodLabels = {
    creditCard: { icon: "bi-credit-card-2-front", label: "Tarjeta de crédito" },
    mercadoPago: { icon: "bi-wallet2", label: "Mercado Pago" },
    transfer: { icon: "bi-bank", label: "Transferencia" },
  };
  const method = methodLabels[paymentMethod] || methodLabels.creditCard;

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <div className="text-center mb-4">
          <div
            className="d-inline-flex align-items-center justify-content-center rounded-circle mb-3"
            style={{
              width: 80,
              height: 80,
              background: "var(--color-1)",
            }}
          >
            <i className="bi bi-check-lg text-white" style={{ fontSize: 48 }}></i>
          </div>
          <h2>¡Reserva confirmada!</h2>
          <p className="text-muted">
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
                <span className="text-muted">N° de reserva</span>
                <p className="mb-0 font-monospace small">{reservation.id}</p>
              </div>
              <div className="col-6 text-end">
                <span className="text-muted">Fecha</span>
                <p className="mb-0">{new Date(reservation.reservedAt).toLocaleString("es-AR")}</p>
              </div>
            </div>

            <hr />

            <h6 className="mb-3">Asientos</h6>
            <ul className="list-unstyled">
              {reservation.items.map(item => (
                <li key={item.id} className="d-flex justify-content-between mb-2">
                  <span>
                    <i className="bi bi-ticket-perforated me-2" style={{ color: "var(--color-3)" }}></i>
                    {item.sectorName} - Fila {item.rowIdentifier}, Asiento {item.seatNumber}
                  </span>
                  <span className="text-muted">
                    ${item.unitPrice.toLocaleString("es-AR")}
                  </span>
                </li>
              ))}
            </ul>

            <hr />

            <div className="row">
              <div className="col-6">
                <p className="mb-1 small text-muted">Método de pago</p>
                <p className="mb-0">
                  <i className={`bi ${method.icon} me-2`}></i>
                  {method.label}
                </p>
              </div>
              <div className="col-6 text-end">
                <p className="mb-1 small text-muted">Total pagado</p>
                <p className="mb-0 fs-4 fw-bold">
                  ${reservation.totalAmount.toLocaleString("es-AR")}
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