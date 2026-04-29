import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

export default function EventDetailsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    api.get(`/api/v1/events/${eventId}`)
      .then(setEvent)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [eventId]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  if (error) {
    return <div className="alert alert-danger">Error: {error}</div>;
  }

  if (!event) {
    return <div className="alert alert-warning">Evento no encontrado.</div>;
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
        <div className="card-body">
          <h1 className="card-title">{event.name}</h1>

          <div className="row mt-3 mb-4">
            <div className="col-md-6">
              <p className="mb-2">
                <i className="bi bi-calendar-event text-primary me-2"></i>
                <strong>Fecha:</strong>{" "}
                {new Date(event.eventDate).toLocaleString("es-AR")}
              </p>
              {event.venue && (
                <p className="mb-2">
                  <i className="bi bi-geo-alt text-primary me-2"></i>
                  <strong>Lugar:</strong> {event.venue}
                </p>
              )}
              {event.status && (
                <p className="mb-2">
                  <i className="bi bi-info-circle text-primary me-2"></i>
                  <strong>Estado:</strong>{" "}
                  <span className="badge bg-success">{event.status}</span>
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
            className="btn btn-primary btn-lg mt-3"
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