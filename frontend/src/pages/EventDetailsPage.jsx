import { useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useEvent } from "../hooks/useEvent";
import { toast } from "../components/toast";
import LoadingState from "../components/LoadingState";
import ErrorState from "../components/ErrorState";
import { formatDateTime } from "../lib/format";

export default function EventDetailsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const { event, loading, error, refetch } = useEvent(eventId);

  // Diferenciamos 404 (aviso) del resto (error). El hook solo trae el error.
  useEffect(() => {
    if (!error) return;
    if (error.status === 404) {
      toast.warning("Evento no encontrado");
    } else {
      toast.error(`Error al cargar el evento: ${error.message}`);
    }
  }, [error]);

  if (loading) {
    return <LoadingState />;
  }

  if (error || !event) {
    const isNotFound = error?.status === 404 || !event;
    return (
      <ErrorState
        message={isNotFound ? "Evento no encontrado." : "No pudimos cargar el evento."}
        actions={
          <>
            <button
              className="btn btn-outline-primary"
              onClick={() => navigate("/events")}
            >
              <i className="bi bi-arrow-left me-2"></i>
              Volver a eventos
            </button>
            {!isNotFound && (
              <button
                className="btn btn-primary"
                onClick={() => refetch().catch(() => {})}
              >
                <i className="bi bi-arrow-clockwise me-2"></i>
                Reintentar
              </button>
            )}
          </>
        }
      />
    );
  }

  return (
    <div>
      <button
        className="btn btn-link text-decoration-none mb-3 p-0"
        onClick={() => navigate("/events")}
      >
        <i className="bi bi-arrow-left me-1"></i> Volver a eventos
      </button>

      <div className="card shadow-sm">
        <div className="card-body p-4 p-md-5">
          <h1 className="card-title h2 mb-4">{event.name}</h1>

          <div className="row g-3 mb-4">
            <div className="col-12 col-md-6">
              <p className="mb-2">
                <i
                  className="bi bi-calendar-event me-2"
                  style={{ color: "var(--color-3)" }}
                ></i>
                <strong>Fecha:</strong> {formatDateTime(event.eventDate)}
              </p>
              {event.venue && (
                <p className="mb-2">
                  <i
                    className="bi bi-geo-alt me-2"
                    style={{ color: "var(--color-3)" }}
                  ></i>
                  <strong>Lugar:</strong> {event.venue}
                </p>
              )}
              {event.status && (
                <p className="mb-2">
                  <i
                    className="bi bi-info-circle me-2"
                    style={{ color: "var(--color-3)" }}
                  ></i>
                  <strong>Estado:</strong>{" "}
                  <span className="badge bg-primary">{event.status}</span>
                </p>
              )}
            </div>
          </div>

          {event.description && (
            <>
              <h5>Descripción</h5>
              <p className="text-overridden-muted">{event.description}</p>
            </>
          )}

          <button
            className="btn btn-primary btn-lg w-100 w-md-auto mt-3"
            onClick={() => navigate(`/events/${eventId}/seats`)}
          >
            <i className="bi bi-grid-3x3-gap me-2"></i>
            Ver mapa de asientos
          </button>
        </div>
      </div>
    </div>
  );
}