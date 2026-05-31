import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useEvents } from "../hooks/useEvents";
import { toast } from "../components/toast";
import LoadingState from "../components/LoadingState";
import EmptyState from "../components/EmptyState";
import { formatDateTime } from "../lib/format";

const CARD_DATE_OPTIONS = {
  day: "2-digit",
  month: "long",
  year: "numeric",
  hour: "2-digit",
  minute: "2-digit",
};

export default function EventsPage() {
  const { data, events, page, nextPage, prevPage, loading, error } = useEvents();

  // El hook expone el error; la Page decide cómo comunicarlo (toast).
  useEffect(() => {
    if (error) toast.error("Error al cargar los eventos.");
  }, [error]);

  if (loading && !data) {
    return <LoadingState message="Cargando eventos..." />;
  }

  return (
    <div>
      <div className="d-flex align-items-center mb-4">
        <i
          className="bi bi-calendar-event fs-3 me-3"
          style={{ color: "var(--color-3)" }}
        ></i>
        <div>
          <h2 className="mb-0">Eventos</h2>
          <small className="text-overridden-muted">
            {data?.totalCount || 0} evento(s) disponibles
          </small>
        </div>
      </div>

      {events.length === 0 ? (
        <EmptyState icon="bi-calendar-x" message="No hay eventos disponibles." />
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
                      {formatDateTime(evt.eventDate, CARD_DATE_OPTIONS)}
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

          {data && data.totalPages > 1 && (
            <nav className="mt-4 d-flex justify-content-center align-items-center gap-3">
              <button
                className="btn btn-outline-light btn-sm"
                disabled={!data.hasPreviousPage || loading}
                onClick={prevPage}
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
                onClick={nextPage}
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