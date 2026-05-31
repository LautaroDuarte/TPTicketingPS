import { useEffect, useState } from "react";
import { api } from "../api/client";
import { toast } from "../components/toast";
import { useParams, useNavigate, useLocation } from "react-router-dom";

export default function PaymentPage() {
  const { reservationId } = useParams();
  const navigate = useNavigate();
  const [reservation, setReservation] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [paymentMethod, setPaymentMethod] = useState("creditCard");
  const [processing, setProcessing] = useState(false);
  const [secondsLeft, setSecondsLeft] = useState(0);
  const [cardHolder, setCardHolder] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [expirationDate, setExpirationDate] = useState("");
  const [cvv, setCvv] = useState("");
  const [showCancelModal, setShowCancelModal] = useState(false);
  const [cancelling, setCancelling] = useState(false);

  useEffect(() => {
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

  // Countdown del timer (visual, basado en lo que devolvió el backend)
  useEffect(() => {
    if (secondsLeft <= 0) return;
    const timer = setInterval(() => {
      setSecondsLeft(s => Math.max(0, s - 1));
    }, 1000);
    return () => clearInterval(timer);
  }, [secondsLeft]);

  const handlePayment = async () => {
  setProcessing(true);

  try {
    const receipt = await api.post(
      `/api/v1/reservations/${reservationId}/payments`,
      {
        paymentMethod,
        cardHolder,
        cardNumber,
        expirationDate,
        cvv,
      }
    );

    toast.success("¡Pago confirmado!");
    navigate(`/reservations/${reservationId}/confirmation`, {
      state: { paymentMethod, reservation, receipt },
    });
  } catch (err) {
    if (err.status === 409) {
      toast.warning(err.message || "No se pudo procesar el pago.");
    } else if (err.status === 400 && err.details) {
      const messages = Object.values(err.details).flat().join(" ");
      toast.error(messages || "Datos de pago inválidos.");
    } else {
      toast.error(err.message || "Error al procesar el pago.");
    }
  } finally {
    setProcessing(false);
  }
};

const handleCancelReservation = async () => {
  setCancelling(true);
  try {
    await api.delete(`/api/v1/reservations/${reservationId}`);
    toast.success("Reserva cancelada. Los asientos fueron liberados.");
    setShowCancelModal(false);
    navigate(`/events`);
  } catch (err) {
    toast.error(err.message || "Error al cancelar la reserva.");
  } finally {
    setCancelling(false);
  }
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
                  <span className="text-overridden-muted">
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
              {paymentMethod === "creditCard" && (
  <div className="row g-3">
    <div className="col-12">
      <label className="form-label small">Número de tarjeta</label>
      <input
        type="text"
        className="form-control"
        placeholder="4111 1111 1111 1111"
        value={cardNumber}
        onChange={(e) => setCardNumber(e.target.value)}
      />
    </div>
    <div className="col-12">
      <label className="form-label small">Titular</label>
      <input
        type="text"
        className="form-control"
        placeholder="Como figura en la tarjeta"
        value={cardHolder}
        onChange={(e) => setCardHolder(e.target.value)}
      />
    </div>
    <div className="col-6">
      <label className="form-label small">Vencimiento</label>
      <input
        type="text"
        className="form-control"
        placeholder="MM/AA"
        value={expirationDate}
        onChange={(e) => setExpirationDate(e.target.value)}
      />
    </div>
    <div className="col-6">
      <label className="form-label small">CVV</label>
      <input
        type="text"
        className="form-control"
        placeholder="123"
        value={cvv}
        onChange={(e) => setCvv(e.target.value)}
      />
    </div>
  </div>
)}
              {paymentMethod === "mercadoPago" && (() => {
  // Seteamos datos default para que el backend no se queje
  if (!cardHolder) setCardHolder("MercadoPago User");
  if (!cardNumber) setCardNumber("4111111111111111");
  if (!expirationDate) setExpirationDate("12/26");
  if (!cvv) setCvv("000");
  return (
    <div className="alert alert-info mb-0">
      <p className="mb-0 small">
        Serás redirigido a Mercado Pago para completar el pago.
      </p>
    </div>
  );
})()}
              {paymentMethod === "transfer" && <TransferForm />}
            </div>
          </div>
        </div>

        <div className="d-flex gap-2 justify-content-end flex-wrap">
          <button
            className="btn btn-outline-secondary"
            onClick={() => setShowCancelModal(true)}
            disabled={processing}
          >
            <i className="bi bi-x-lg me-1"></i>
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
      
      {showCancelModal && (
        <div className="modal d-block" tabIndex={-1} style={{ background: "rgba(0,0,0,0.6)" }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content text-white" style={{ background: "var(--color-6)" }}>
              <div className="modal-header border-0">
                <h5 className="modal-title">
                  <i className="bi bi-exclamation-triangle text-warning me-2"></i>
                  Cancelar reserva
                </h5>
                <button
                  type="button"
                  className="btn-close btn-close-white"
                  onClick={() => setShowCancelModal(false)}
                  disabled={cancelling}
                ></button>
              </div>
              <div className="modal-body">
                <p className="mb-2">
                  ¿Estás seguro de que querés cancelar tu reserva?
                </p>
                <p className="small mb-0" style={{ opacity: 0.8 }}>
                  Los asientos seleccionados serán liberados inmediatamente y
                  cualquier otro usuario podrá reservarlos.
                </p>
              </div>
              <div className="modal-footer border-0">
                <button
                  className="btn btn-outline-light"
                  onClick={() => setShowCancelModal(false)}
                  disabled={cancelling}
                >
                  Volver al pago
                </button>
                <button
                  className="btn btn-danger"
                  onClick={handleCancelReservation}
                  disabled={cancelling}
                >
                  {cancelling ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-2"></span>
                      Cancelando...
                    </>
                  ) : (
                    <>
                      <i className="bi bi-trash me-1"></i>
                      Sí, cancelar reserva
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
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
          <small className="text-overridden-muted">{description}</small>
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
        <p className="text-overridden-muted small mb-0">
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