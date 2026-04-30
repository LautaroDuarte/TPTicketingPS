import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
<<<<<<< Updated upstream
=======
import { toast } from "../components/toast";
>>>>>>> Stashed changes

export default function PaymentPage() {
  const { reservationId } = useParams();
  const navigate = useNavigate();
  const [reservation, setReservation] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [paymentMethod, setPaymentMethod] = useState("creditCard");
  const [processing, setProcessing] = useState(false);
  const [secondsLeft, setSecondsLeft] = useState(0);

  useEffect(() => {
<<<<<<< Updated upstream
    api.get(`/api/v1/reservations/${reservationId}`)
      .then(data => {
        setReservation(data);
        setSecondsLeft(data.secondsRemaining || 0);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [reservationId]);

  // Countdown del timer (visual, basado en lo que devolvió el backend)
  useEffect(() => {
=======
>>>>>>> Stashed changes
  api.get(`/api/v1/reservations/${reservationId}`)
    .then(data => {
      setReservation(data);
      setSecondsLeft(data.secondsRemaining || 0);
    })
    .catch(err => {
      setError(err.message);
      toast.error(`Error al cargar reserva: ${err.message}`);
    })
    .finally(() => setLoading(false));
}, [reservationId]);

<<<<<<< Updated upstream
=======
  // Countdown del timer (visual, basado en lo que devolvió el backend)
  useEffect(() => {
    if (secondsLeft <= 0) return;
    const timer = setInterval(() => {
      setSecondsLeft(s => Math.max(0, s - 1));
    }, 1000);
    return () => clearInterval(timer);
  }, [secondsLeft]);

>>>>>>> Stashed changes
  const handlePayment = async () => {
  setProcessing(true);
  toast.info("Procesando pago...");

  setTimeout(() => {
    toast.success("¡Pago confirmado!");
    navigate(`/reservations/${reservationId}/confirmation`, {
      state: { paymentMethod, reservation }
    });
  }, 1500);
};

  const formatTime = (s) => {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${String(m).padStart(2, "0")}:${String(sec).padStart(2, "0")}`;
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary"></div>
      </div>
    );
  }

  if (error) return <div className="alert alert-danger">Error: {error}</div>;
  if (!reservation) return null;

  const expired = secondsLeft === 0;

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <h2 className="mb-3">
          <i className="bi bi-credit-card me-2"></i>Confirmar pago
        </h2>

        {/* Timer */}
        <div className={`alert ${expired ? "alert-danger" : "alert-info"} d-flex align-items-center justify-content-between`}>
          <div>
            <i className="bi bi-clock me-2"></i>
            {expired ? (
              <strong>Tu reserva ha expirado.</strong>
            ) : (
              <>
                Tiempo restante: <strong>{formatTime(secondsLeft)}</strong>
              </>
            )}
          </div>
        </div>

        {/* Resumen */}
        <div className="card shadow-sm mb-4">
          <div className="card-header">
            <strong>Detalle de la reserva</strong>
          </div>
          <div className="card-body">
            <ul className="list-unstyled mb-3">
              {reservation.items.map(item => (
                <li key={item.id} className="d-flex justify-content-between mb-1">
                  <span>
                    {item.sectorName} - Fila {item.rowIdentifier}, Asiento {item.seatNumber}
                  </span>
                  <span className="text-muted">
                    ${item.unitPrice.toLocaleString("es-AR")}
                  </span>
                </li>
              ))}
            </ul>
            <div className="d-flex justify-content-between fw-bold border-top pt-2 fs-5">
              <span>Total:</span>
              <span>${reservation.totalAmount.toLocaleString("es-AR")}</span>
            </div>
          </div>
        </div>

        {/* Selector de método */}
        <div className="card shadow-sm mb-4">
          <div className="card-header">
            <strong>Método de pago</strong>
          </div>
          <div className="card-body">
            <div className="row g-3">
              <PaymentOption
                value="creditCard"
                selected={paymentMethod}
                onSelect={setPaymentMethod}
                icon="bi-credit-card-2-front"
                label="Tarjeta de crédito"
                description="Visa, Mastercard, Amex"
              />
              <PaymentOption
                value="mercadoPago"
                selected={paymentMethod}
                onSelect={setPaymentMethod}
                icon="bi-wallet2"
                label="Mercado Pago"
                description="Saldo o tarjetas asociadas"
              />
              <PaymentOption
                value="transfer"
                selected={paymentMethod}
                onSelect={setPaymentMethod}
                icon="bi-bank"
                label="Transferencia"
                description="CBU / Alias"
              />
            </div>

            {/* Form dinámico según método (solo simulación visual) */}
            <div className="mt-4">
              {paymentMethod === "creditCard" && <CreditCardForm />}
              {paymentMethod === "mercadoPago" && <MercadoPagoForm />}
              {paymentMethod === "transfer" && <TransferForm />}
            </div>
          </div>
        </div>

        <div className="d-flex gap-2 justify-content-end flex-wrap">
          <button
            className="btn btn-outline-primary"
            onClick={() => navigate(-1)}
            disabled={processing}
          >
            Cancelar
          </button>
          <button
            className="btn btn-primary btn-lg"
            onClick={handlePayment}
            disabled={processing || expired}
          >
            {processing ? (
              <>
                <span className="spinner-border spinner-border-sm me-2"></span>
                Procesando...
              </>
            ) : (
              <>
                <i className="bi bi-check-circle me-2"></i>
                Pagar ${reservation.totalAmount.toLocaleString("es-AR")}
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  );
}

function PaymentOption({ value, selected, onSelect, icon, label, description }) {
  const isSelected = value === selected;
  return (
    <div className="col-12 col-md-4">
      <div
        className={`card h-100 ${isSelected ? "border-primary" : ""}`}
        onClick={() => onSelect(value)}
        style={{ cursor: "pointer", borderWidth: 2 }}
      >
        <div className="card-body text-center">
          <i className={`bi ${icon} fs-1 mb-2`} style={{ color: isSelected ? "var(--color-3)" : "var(--text-muted)" }}></i>
          <h6 className="mb-1">{label}</h6>
          <small className="text-muted">{description}</small>
        </div>
      </div>
    </div>
  );
}

function CreditCardForm() {
  return (
    <div className="row g-3">
      <div className="col-12">
        <label className="form-label small">Número de tarjeta</label>
        <input type="text" className="form-control" placeholder="1234 5678 9012 3456" />
      </div>
      <div className="col-12">
        <label className="form-label small">Titular</label>
        <input type="text" className="form-control" placeholder="Como figura en la tarjeta" />
      </div>
      <div className="col-6">
        <label className="form-label small">Vencimiento</label>
        <input type="text" className="form-control" placeholder="MM/AA" />
      </div>
      <div className="col-6">
        <label className="form-label small">CVV</label>
        <input type="text" className="form-control" placeholder="123" />
      </div>
    </div>
  );
}

function MercadoPagoForm() {
  return (
    <div className="row g-3">
      <div className="col-12">
        <label className="form-label small">Email de Mercado Pago</label>
        <input type="email" className="form-control" placeholder="tu@email.com" />
      </div>
      <div className="col-12">
        <p className="text-muted small mb-0">
          Vas a ser redirigido a Mercado Pago para completar el pago.
        </p>
      </div>
    </div>
  );
}

function TransferForm() {
  return (
    <div className="alert alert-info mb-0">
      <p className="mb-2"><strong>Datos para transferencia:</strong></p>
      <p className="mb-1 small">CBU: 0000003100012345678901</p>
      <p className="mb-1 small">Alias: TICKETING.RESERVA</p>
      <p className="mb-0 small">Una vez confirmada, tu reserva será procesada.</p>
    </div>
  );
}