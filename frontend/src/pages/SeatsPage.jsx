import { useNavigate, useParams } from "react-router-dom";
import { useSeatMap } from "../hooks/useSeatMap";
import { useSeatSelection } from "../hooks/useSeatSelection";
import { useCreateReservation } from "../hooks/useCreateReservation";
import { useAuth } from "../hooks/useAuth";
import { toast } from "../components/toast";
import { parseApiError } from "../lib/errors";
import LoadingState from "../components/LoadingState";
import ErrorState from "../components/ErrorState";
import SeatMap from "../components/seats/SeatMap";
import SeatLegend from "../components/seats/SeatLegend";
import ReservationSummary from "../components/seats/ReservationSummary";
import "../components/seats/SeatsPage.css";

export default function SeatsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();

  const { data, loading, error, refetch } = useSeatMap(eventId);
  const { selectedSeats, toggleSeat, isSelected, clear, totalAmount } =
    useSeatSelection();
  const { createReservation, submitting } = useCreateReservation();
  const { isAuthenticated } = useAuth();

  const handleConfirmReservation = async () => {
    if (selectedSeats.length === 0) return;

    if (!isAuthenticated) {
      toast.warning("Tenés que identificarte primero. Configurá tu usuario.");
      return;
    }

    try {
      const reservation = await createReservation({
        eventId,
        seatIds: selectedSeats.map(s => s.id),
      });
      toast.success("Reserva creada. Tenés 5 minutos para pagar.");
      navigate(`/reservations/${reservation.id}/payment`);
    } catch (err) {
      handleReservationError(err);
    }
  };

  // Mapeo de error -> UX. Es lógica de presentación (qué toast, refrescar el
  // plano, limpiar selección), por eso vive en la Page y no en el hook.
  const resyncSeats = () => {
    refetch(true).catch(() => {}); // refresco silencioso, sin spinner
    clear();
  };

  const handleReservationError = (err) => {
    const message = err.message;
    switch (err.status) {
      case 409:
        toast.warning(message || "Conflicto al crear la reserva.");
        if (message?.toLowerCase().includes("asientos")) resyncSeats();
        break;
      case 404:
        toast.error(message || "Recurso no encontrado.");
        resyncSeats();
        break;
      case 400:
        toast.error(parseApiError(err, "Datos inválidos."));
        break;
      default:
        //toast.error(message || "Error inesperado al crear la reserva.");
        console.error(err);
    }
  };

  if (loading) {
    return <LoadingState />;
  }

  if (error && !data) {
    return (
      <ErrorState
        message="No pudimos cargar el mapa de asientos."
        actions={
          <button className="btn btn-primary" onClick={() => refetch()}>
            <i className="bi bi-arrow-clockwise me-2"></i>
            Reintentar
          </button>
        }
      />
    );
  }

  if (!data) return null;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <button
          className="btn btn-link text-decoration-none p-0"
          onClick={() => navigate(`/events/${eventId}`)}
        >
          <i className="bi bi-arrow-left me-1"></i> Volver al evento
        </button>
        <button
          className="btn btn-outline-primary btn-sm"
          onClick={() => refetch()}
          disabled={loading}
        >
          <i className="bi bi-arrow-clockwise me-1"></i>
          Actualizar
        </button>
      </div>

      <h2 className="mb-3">{data.eventName}</h2>

      {!isAuthenticated && (
        <div className="alert alert-warning d-flex align-items-center">
          <i className="bi bi-exclamation-triangle me-2 fs-5"></i>
          <div className="flex-grow-1">
            <strong>No estás identificado.</strong>
            <br />
            <small>
              Para hacer una reserva necesitás{" "}
              <a href="/login" className="alert-link">
                iniciar sesión
              </a>
              .
            </small>
          </div>
        </div>
      )}

      <div className="d-flex gap-3 mb-4 flex-wrap">
        <SeatLegend className="available" label="Disponible" />
        <SeatLegend className="selected" label="Seleccionado" />
        <SeatLegend className="reserved" label="Reservado" />
        <SeatLegend className="sold" label="Vendido" />
      </div>

      <div className="row g-4">
        <div className="col-12 col-lg-9">
          <SeatMap
            sectors={data.sectors}
            isSelected={isSelected}
            onToggle={toggleSeat}
            disabled={submitting}
          />
        </div>

        <div className="col-12 col-lg-3">
          <ReservationSummary
            selectedSeats={selectedSeats}
            totalAmount={totalAmount}
            onConfirm={handleConfirmReservation}
            submitting={submitting}
            disabled={!isAuthenticated}
          />
        </div>
      </div>
    </div>
  );
}