import { useParams, useNavigate } from "react-router-dom";
import { useReservation } from "../hooks/useReservation";
import { useCountdown } from "../hooks/useCountdown";
import { usePayment } from "../hooks/usePayment";
import { useCancelReservation } from "../hooks/useCancelReservation";
import { useCardForm } from "../hooks/useCardForm";
import { useState } from "react";
import { toast } from "../components/toast";
import { parseApiError } from "../lib/errors";
import { formatCurrency } from "../lib/format";
import { MERCADO_PAGO_DEFAULTS } from "../constants/paymentMethods";
import { TRANSFER_DEFAULTS } from "../constants/paymentMethods";
import LoadingState from "../components/LoadingState";
import Spinner from "../components/Spinner";
import ConfirmModal from "../components/ConfirmModal";
import CountdownAlert from "../components/payment/CountdownAlert";
import PaymentMethodSelector from "../components/payment/PaymentMethodSelector";
import ReservationItems from "../components/reservations/ReservationItems";

export default function PaymentPage() {
  const { reservationId } = useParams();
  const navigate = useNavigate();

  const { reservation, loading, error } = useReservation(reservationId);
  // El timer arranca con lo que devolvió el backend; useCountdown se
  // re-sincroniza solo cuando llega ese valor (ver el hook).
  const { secondsLeft, expired } = useCountdown(reservation?.secondsRemaining ?? 0);

  const { pay, processing } = usePayment(reservationId);
  const { cancel, cancelling } = useCancelReservation(reservationId);
  const { card, setField } = useCardForm();

  const [paymentMethod, setPaymentMethod] = useState("creditCard"); 
  const [showCancelModal, setShowCancelModal] = useState(false);

  // Construimos el payload según el método. Para Mercado Pago usamos datos
  // por defecto en vez de tocar el formulario durante el render.
  const buildPayload = () => {
    if (paymentMethod === "mercadoPago") {
      return { paymentMethod, ...MERCADO_PAGO_DEFAULTS };
    }
    if (paymentMethod === "transfer") {
      return { paymentMethod, ...TRANSFER_DEFAULTS };
    }
    return { paymentMethod, ...card };
  };

  const handlePayment = async () => {
    try {
      const receipt = await pay(buildPayload());
      toast.success("¡Pago confirmado!");
      navigate(`/reservations/${reservationId}/confirmation`, {
        state: { paymentMethod, reservation, receipt },
      });
    } catch (err) {
      if (err.status === 409) {
        toast.warning(err.message || "No se pudo procesar el pago.");
      } else if (err.status === 400) {
        toast.error(parseApiError(err, "Datos de pago inválidos."));
      } else {
        toast.error(err.message || "Error al procesar el pago.");
      }
    }
  };

  const handleCancelReservation = async () => {
    try {
      await cancel();
      toast.success("Reserva cancelada. Los asientos fueron liberados.");
      setShowCancelModal(false);
      navigate("/events");
    } catch (err) {
      toast.error(err.message || "Error al cancelar la reserva.");
    }
  };

  if (loading) {
    return <LoadingState />;
  }

  if (error) {
    return <div className="alert alert-danger">Error: {error.message}</div>;
  }

  if (!reservation) return null;

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <h2 className="mb-3">
          <i className="bi bi-credit-card me-2"></i>Confirmar pago
        </h2>

        <CountdownAlert secondsLeft={secondsLeft} expired={expired} />

        {/* Resumen de la reserva */}
        <div className="card shadow-sm mb-4">
          <div className="card-header">
            <strong>Detalle de la reserva</strong>
          </div>
          <div className="card-body">
            <ReservationItems items={reservation.items} />
            <div className="d-flex justify-content-between fw-bold border-top pt-2 fs-5">
              <span>Total:</span>
              <span>{formatCurrency(reservation.totalAmount)}</span>
            </div>
          </div>
        </div>

        <PaymentMethodSelector
          selected={paymentMethod}
          onSelect={setPaymentMethod}
          card={card}
          onCardField={setField}
        />

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
                <Spinner small className="me-2" />
                Procesando...
              </>
            ) : (
              <>
                <i className="bi bi-check-circle me-2"></i>
                Pagar {formatCurrency(reservation.totalAmount)}
              </>
            )}
          </button>
        </div>
      </div>

      <ConfirmModal
        open={showCancelModal}
        title="Cancelar reserva"
        confirmLabel="Sí, cancelar reserva"
        cancelLabel="Volver al pago"
        loading={cancelling}
        onConfirm={handleCancelReservation}
        onCancel={() => setShowCancelModal(false)}
      >
        <p className="mb-2">¿Estás seguro de que querés cancelar tu reserva?</p>
        <p className="small mb-0" style={{ opacity: 0.8 }}>
          Los asientos seleccionados serán liberados inmediatamente y cualquier
          otro usuario podrá reservarlos.
        </p>
      </ConfirmModal>
    </div>
  );
}