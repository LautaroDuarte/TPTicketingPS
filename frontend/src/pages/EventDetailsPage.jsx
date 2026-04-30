import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";
import { toast } from "../components/toast";

export default function EventDetailsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [hasError, setHasError] = useState(false);

  const fetchEvent = () => {
    setLoading(true);
    setHasError(false);

    api.get(`/api/v1/events/${eventId}`)
      .then(setEvent)
      .catch(err => {
        setHasError(true);
        if (err.status === 404) {
          toast.warning("Evento no encontrado");
        } else {
          toast.error(`Error al cargar el evento: ${err.message}`);
        }
      })
      .finally(() => setLoading(false));
  };

  useEffect(fetchEvent, [eventId]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  if (hasError || !event) {
    return (
      <div className="text-center py-5">
        <i className="bi bi-exclamation-circle fs-1 text-warning"></i>
        <p className="mt-3 mb-3">
          {hasError ? "No pudimos cargar el evento." : "Evento no encontrado."}
        </p>
        <button
          className="btn btn-outline-primary me-2"
          onClick={() => navigate("/events")}
        >
          <i className="bi bi-arrow-left me-2"></i>
          Volver a eventos
        </button>
        {hasError && (
          <button className="btn btn-primary" onClick={fetchEvent}>
            <i className="bi bi-arrow-clockwise me-2"></i>
            Reintentar
          </button>
        )}
      </div>
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
                <i className="bi bi-calendar-event me-2" style={{ color: "var(--color-3)" }}></i>
                <strong>Fecha:</strong>{" "}
                {new Date(event.eventDate).toLocaleString("es-AR")}
              </p>
              {event.venue && (
                <p className="mb-2">
                  <i className="bi bi-geo-alt me-2" style={{ color: "var(--color-3)" }}></i>
                  <strong>Lugar:</strong> {event.venue}
                </p>
              )}
              {event.status && (
                <p className="mb-2">
                  <i className="bi bi-info-circle me-2" style={{ color: "var(--color-3)" }}></i>
                  <strong>Estado:</strong>{" "}
                  <span className="badge bg-primary">{event.status}</span>
                </p>
              )}
            </div>
          </div>

          {event.description && (
            <>
              <h5>Descripción</h5>
              <p className="text-muted">{event.description}</p>
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