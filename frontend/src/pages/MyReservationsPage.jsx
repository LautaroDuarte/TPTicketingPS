import { useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useMyReservations } from "../hooks/useMyReservations";
import { toast } from "../components/toast";
import LoadingState from "../components/LoadingState";
import EmptyState from "../components/EmptyState";
import ReservationCard from "../components/reservations/ReservationCard";

export default function MyReservationsPage() {
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuth();
  const { reservations, loading, error } = useMyReservations(user.id);

  // Sin sesión no hay nada que mostrar: al login.
  useEffect(() => {
    if (!isAuthenticated) navigate("/login");
  }, [isAuthenticated, navigate]);

  useEffect(() => {
    if (error) toast.error("Error al cargar las reservas.");
  }, [error]);

  if (loading) {
    return <LoadingState message="Cargando reservas..." />;
  }

  return (
    <div>
      <div className="d-flex align-items-center mb-4">
        <i className="bi bi-ticket fs-3 me-3" style={{ color: "var(--color-3)" }}></i>
        <div>
          <h2 className="mb-0">Mis reservas</h2>
          <small className="text-overridden-muted">
            {reservations.length} reserva(s)
          </small>
        </div>
      </div>

      {reservations.length === 0 ? (
        <EmptyState
          message="No tenés reservas todavía."
          action={
            <Link className="btn btn-primary" to="/events">
              <i className="bi bi-search me-1"></i>
              Explorar eventos
            </Link>
          }
        />
      ) : (
        <div className="row g-3">
          {reservations.map((reservation) => (
            <div key={reservation.id} className="col-12">
              <ReservationCard reservation={reservation} />
            </div>
          ))}
        </div>
      )}
    </div>
  );
}