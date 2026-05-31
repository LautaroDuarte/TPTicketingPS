import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../api/client";
import { toast } from "../components/toast";

const statusConfig = {
  Pending: { label: "Pendiente", class: "bg-warning text-dark", icon: "bi-clock" },
  Paid: { label: "Pagada", class: "bg-success", icon: "bi-check-circle" },
  Expired: { label: "Expirada", class: "bg-secondary", icon: "bi-x-circle" },
  Cancelled: { label: "Cancelada", class: "bg-danger", icon: "bi-slash-circle" },
};

export default function MyReservationsPage() {
  const navigate = useNavigate();
  const userId = localStorage.getItem("userId");
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!userId) {
      navigate("/login");
      return;
    }
    loadReservations();
  }, [userId]);

  const loadReservations = async () => {
    try {
      const data = await api.get(`/api/v1/users/${userId}/reservations`);
      setReservations(data);
    } catch (err) {
      toast.error("Error al cargar las reservas.");
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr) => {
    return new Date(dateStr).toLocaleString("es-AR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const getTimeRemaining = (expiresAt) => {
    const diff = new Date(expiresAt) - new Date();
    if (diff <= 0) return "Expirada";
    const mins = Math.floor(diff / 60000);
    const secs = Math.floor((diff % 60000) / 1000);
    return `${mins}:${secs.toString().padStart(2, "0")}`;
  };

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border" role="status"></div>
        <p className="mt-2">Cargando reservas...</p>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex align-items-center mb-4">
        <i className="bi bi-ticket fs-3 me-3" style={{ color: "var(--color-3)" }}></i>
        <div>
          <h2 className="mb-0">Mis reservas</h2>
          <small className="text-overridden-muted">{reservations.length} reserva(s)</small>
        </div>
      </div>

      {reservations.length === 0 ? (
        <div className="text-center py-5">
          <i className="bi bi-inbox fs-1 text-overridden-muted d-block mb-2"></i>
          <p className="text-overridden-muted">No tenés reservas todavía.</p>
          <Link className="btn btn-primary" to="/events">
            <i className="bi bi-search me-1"></i>
            Explorar eventos
          </Link>
        </div>
      ) : (
        <div className="row g-3">
          {reservations.map((r) => {
            const config = statusConfig[r.status] || statusConfig.Pending;
            const isPending = r.status === "Pending";
            const isExpiredByTime = isPending && new Date(r.expiresAt) <= new Date();

            return (
              <div key={r.id} className="col-12">
                <div className="card">
                  <div className="card-body">
                    <div className="row align-items-center">
                      {/* Info principal */}
                      <div className="col-md-4">
                        <div className="d-flex align-items-center gap-2 mb-1">
                          <span className={`badge ${config.class}`}>
                            <i className={`bi ${config.icon} me-1`}></i>
                            {config.label}
                          </span>
                          {isPending && !isExpiredByTime && (
                            <span className="badge bg-info text-dark">
                              <i className="bi bi-stopwatch me-1"></i>
                              {getTimeRemaining(r.expiresAt)}
                            </span>
                          )}
                        </div>
                        <small className="text-overridden-muted d-block">
                          Reservada el {formatDate(r.reservedAt)}
                        </small>
                        <small className="text-overridden-muted d-block">
                          ID: <code className="small">{r.id.slice(0, 8)}...</code>
                        </small>
                      </div>

                      {/* Asientos */}
                      <div className="col-md-4">
                        <small className="text-overridden-muted d-block mb-1">Asientos:</small>
                        <div className="d-flex flex-wrap gap-1">
                          {(r.items || []).map((item) => (
                            <span
                              key={item.id || item.seatId}
                              className="badge bg-dark"
                              style={{ fontSize: 11 }}
                            >
                              {item.sectorName || "?"} - {item.rowIdentifier}
                              {item.seatNumber}
                            </span>
                          ))}
                        </div>
                      </div>

                      {/* Total y acciones */}
                      <div className="col-md-4 text-md-end mt-2 mt-md-0">
                        <div className="fs-5 fw-bold mb-2">
                          ${r.totalAmount?.toLocaleString("es-AR") || "0"}
                        </div>
                        {isPending && !isExpiredByTime && (
                          <Link
                            className="btn btn-primary btn-sm"
                            to={`/reservations/${r.id}/payment`}
                          >
                            <i className="bi bi-credit-card me-1"></i>
                            Pagar ahora
                          </Link>
                        )}
                        {r.status === "Paid" && (
                          <Link
                            className="btn btn-outline-success btn-sm"
                            to={`/reservations/${r.id}/confirmation`}
                          >
                            <i className="bi bi-receipt me-1"></i>
                            Ver comprobante
                          </Link>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}