import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { api } from "../api/client";
import { toast } from "../components/toast";

export default function EventsPage() {
  const [data, setData] = useState(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const pageSize = 9; // múltiplo de 3 para que la grilla quede pareja

  useEffect(() => {
    loadEvents();
  }, [page]);

  const loadEvents = async () => {
    setLoading(true);
    try {
      const result = await api.get(
        `/api/v1/events?page=${page}&pageSize=${pageSize}&status=Active`
      );
      setData(result);
    } catch (err) {
      toast.error("Error al cargar los eventos.");
    } finally {
      setLoading(false);
    }
  };

  if (loading && !data) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border" role="status"></div>
        <p className="mt-2">Cargando eventos...</p>
      </div>
    );
  }

  const events = data?.items || [];

  return (
    <div>
      <div className="d-flex align-items-center mb-4">
        <i className="bi bi-calendar-event fs-3 me-3" style={{ color: "var(--color-3)" }}></i>
        <div>
          <h2 className="mb-0">Eventos</h2>
          <small className="text-overridden-muted">{data?.totalCount || 0} evento(s) disponibles</small>
        </div>
      </div>

      {events.length === 0 ? (
        <div className="text-center py-5">
          <i className="bi bi-calendar-x fs-1 text-overridden-muted d-block mb-2"></i>
          <p className="text-overridden-muted">No hay eventos disponibles.</p>
        </div>
      ) : (
        <>
          <div className="row g-3">
            {events.map((evt) => (
              <div key={evt.id} className="col-12 col-md-6 col-lg-4">
                <div className="card h-100">
                  <div className="card-body d-flex flex-column">
                    <h5 className="card-title">{evt.name}</h5>
                    <p className="card-text text-overridden-muted small mb-1">
                      <i className="bi bi-geo-alt me-1"></i>
                      {evt.venue}
                    </p>
                    <p className="card-text text-overridden-muted small mb-3">
                      <i className="bi bi-clock me-1"></i>
                      {new Date(evt.eventDate).toLocaleString("es-AR", {
                        day: "2-digit",
                        month: "long",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </p>
                    <Link
                      to={`/events/${evt.id}`}
                      className="btn btn-primary mt-auto"
                    >
                      Ver detalle
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Controles de paginación */}
          {data && data.totalPages > 1 && (
            <nav className="mt-4 d-flex justify-content-center align-items-center gap-3">
              <button
                className="btn btn-outline-light btn-sm"
                disabled={!data.hasPreviousPage || loading}
                onClick={() => setPage((p) => p - 1)}
              >
                <i className="bi bi-chevron-left me-1"></i>
                Anterior
              </button>

              <span className="small text-overridden-muted">
                Página {data.page} de {data.totalPages}
              </span>

              <button
                className="btn btn-outline-light btn-sm"
                disabled={!data.hasNextPage || loading}
                onClick={() => setPage((p) => p + 1)}
              >
                Siguiente
                <i className="bi bi-chevron-right ms-1"></i>
              </button>
            </nav>
          )}
        </>
      )}
    </div>
  );
}