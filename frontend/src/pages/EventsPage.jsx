import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { toast } from "../components/toast";

export default function EventsPage() {
  const navigate = useNavigate();
  const [events, setEvents] = useState([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [hasError, setHasError] = useState(false);

  const fetchEvents = () => {
    setLoading(true);
    setHasError(false);

    api.get(`/api/v1/events?page=${page}&pageSize=10`)
      .then(data => setEvents(Array.isArray(data) ? data : data?.data || []))
      .catch(err => {
        setHasError(true);
        toast.error(`Error al cargar eventos: ${err.message}`);
      })
      .finally(() => setLoading(false));
  };

  useEffect(fetchEvents, [page]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Cargando...</span>
        </div>
      </div>
    );
  }

  if (hasError) {
    return (
      <div className="text-center py-5">
        <i className="bi bi-exclamation-triangle fs-1 text-warning"></i>
        <p className="mt-3 mb-3">No pudimos cargar los eventos.</p>
        <button className="btn btn-primary" onClick={fetchEvents}>
          <i className="bi bi-arrow-clockwise me-2"></i>
          Reintentar
        </button>
      </div>
    );
  }

  return (
    <div>
      <h1 className="mb-4">Catálogo de eventos</h1>

      {events.length === 0 ? (
        <div className="alert alert-info">
          <i className="bi bi-info-circle me-2"></i>
          No hay eventos disponibles en este momento.
        </div>
      ) : (
        <div className="row g-4">
          {events.map(event => (
            <div key={event.id} className="col-12 col-sm-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body d-flex flex-column">
                  <h5 className="card-title">{event.name}</h5>
                  <p className="text-muted mb-2 small">
                    <i className="bi bi-calendar-event me-1"></i>
                    {new Date(event.eventDate).toLocaleDateString("es-AR", {
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                    })}
                  </p>
                  {event.venue && (
                    <p className="text-muted mb-3 small">
                      <i className="bi bi-geo-alt me-1"></i>
                      {event.venue}
                    </p>
                  )}
                  <button
                    className="btn btn-primary mt-auto"
                    onClick={() => navigate(`/events/${event.id}`)}
                  >
                    Ver evento
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="d-flex justify-content-center align-items-center gap-3 mt-4 flex-wrap">
        <button
          className="btn btn-outline-primary"
          onClick={() => setPage(p => Math.max(p - 1, 1))}
          disabled={page === 1}
        >
          <i className="bi bi-chevron-left"></i> Anterior
        </button>
        <span>Página {page}</span>
        <button
          className="btn btn-outline-primary"
          onClick={() => setPage(p => p + 1)}
        >
          Siguiente <i className="bi bi-chevron-right"></i>
        </button>
      </div>
    </div>
  );
}