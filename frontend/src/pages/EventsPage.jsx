import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";

export default function EventsPage() {
  const navigate = useNavigate();
  const [events, setEvents] = useState([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setLoading(true);
    setError(null);

    api.get(`/api/v1/events?page=${page}&pageSize=10`)
      .then(data => {
        // Soporta tanto respuesta paginada { data: [...] } como array directo
        setEvents(Array.isArray(data) ? data : data?.data || []);
      })
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [page]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Cargando...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return <div className="alert alert-danger">Error al cargar eventos: {error}</div>;
  }

  return (
    <div>
      <h1 className="mb-4">Catálogo de eventos</h1>

      {events.length === 0 ? (
        <div className="alert alert-info">No hay eventos disponibles.</div>
      ) : (
        <div className="row g-4">
          {events.map(event => (
            <div key={event.id} className="col-md-6 col-lg-4">
              <div className="card h-100 shadow-sm">
                <div className="card-body d-flex flex-column">
                  <h5 className="card-title">{event.name}</h5>
                  <p className="text-muted mb-2">
                    <i className="bi bi-calendar-event me-1"></i>
                    {new Date(event.eventDate).toLocaleDateString("es-AR", {
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                    })}
                  </p>
                  {event.venue && (
                    <p className="text-muted mb-3">
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

      <div className="d-flex justify-content-center align-items-center gap-3 mt-4">
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