import { useEffect, useMemo, useState, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { MdEventSeat } from "react-icons/md";
import { api } from "../api/client";
import { toast } from "../components/toast";
import "./SeatsPage.css";

export default function SeatsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [hasError, setHasError] = useState(false);
  const [selectedSeats, setSelectedSeats] = useState([]);
  const [submitting, setSubmitting] = useState(false);

  const fetchSeats = useCallback((silent = false) => {
    if (!silent) setLoading(true);
    setHasError(false);

    api.get(`/api/v1/events/${eventId}/seats`)
      .then(setData)
      .catch(err => {
        setHasError(true);
        toast.error(`Error al cargar asientos: ${err.message}`);
      })
      .finally(() => {
        if (!silent) setLoading(false);
      });
  }, [eventId]);

  useEffect(() => {
    fetchSeats();
  }, [fetchSeats]);

  const toggleSeat = (seat, sectorPrice, sectorName) => {
    setSelectedSeats(prev => {
      const exists = prev.find(s => s.id === seat.id);
      if (exists) return prev.filter(s => s.id !== seat.id);
      return [...prev, { ...seat, price: sectorPrice, sectorName }];
    });
  };

  const handleConfirmReservation = async () => {
    if (selectedSeats.length === 0) return;

    // Validación previa: usuario seteado
    const userId = localStorage.getItem("userId");
    if (!userId) {
      toast.warning("Tenés que identificarte primero. Configurá tu usuario.");
      return;
    }

    setSubmitting(true);

    try {
      const reservation = await api.post("/api/v1/reservations", {
        eventId: parseInt(eventId, 10),
        seatIds: selectedSeats.map(s => s.id),
      });

      toast.success(`Reserva creada. Tenés 5 minutos para pagar.`);
      navigate(`/reservations/${reservation.id}/payment`);
    } catch (err) {
      handleReservationError(err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleReservationError = (err) => {
    const backendMessage = err.message;
    if (err.status === 409) {
      toast.warning(backendMessage || "Conflicto al crear la reserva.");
      if (backendMessage?.toLowerCase().includes("asientos")) {
      fetchSeats(true);
      setSelectedSeats([]);
    }
    } else if (err.status === 404) {
      toast.error(backendMessage || "Recurso no encontrado.");
      fetchSeats(true);
      setSelectedSeats([]);
    } else if (err.status === 400) {
      // Validation errors del backend
      if (err.details) {
        const messages = Object.values(err.details).flat().join(" ");
        toast.error(messages || backendMessage || "Datos inválidos.");
      } else {
        toast.error(backendMessage || "Datos inválidos.");
      }
    } else {
      toast.error(backendMessage || "Error inesperado al crear la reserva.");
    }
  };

  // Agrupación
  const processedData = useMemo(() => {
    if (!data) return null;
    return {
      ...data,
      sectors: data.sectors.map(sector => {
        const grouped = sector.seats.reduce((acc, seat) => {
          const row = seat.rowIdentifier;
          if (!acc[row]) acc[row] = [];
          acc[row].push(seat);
          return acc;
        }, {});
        Object.keys(grouped).forEach(row =>
          grouped[row].sort((a, b) => a.seatNumber - b.seatNumber)
        );
        return { ...sector, groupedSeats: grouped };
      }),
    };
  }, [data]);

  if (loading) {
    return (
      <div className="text-center py-5">
        <div className="spinner-border text-primary"></div>
      </div>
    );
  }

  if (hasError && !data) {
    return (
      <div className="text-center py-5">
        <i className="bi bi-exclamation-circle fs-1 text-warning"></i>
        <p className="mt-3 mb-3">No pudimos cargar el mapa de asientos.</p>
        <button className="btn btn-primary" onClick={() => fetchSeats()}>
          <i className="bi bi-arrow-clockwise me-2"></i>
          Reintentar
        </button>
      </div>
    );
  }

  if (!processedData) return null;

  const totalAmount = selectedSeats.reduce((acc, s) => acc + s.price, 0);
  const userId = localStorage.getItem("userId");

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
          onClick={() => fetchSeats()}
          disabled={loading}
        >
          <i className="bi bi-arrow-clockwise me-1"></i>
          Actualizar
        </button>
      </div>

      <h2 className="mb-3">{processedData.eventName}</h2>

      {!userId && (
        <div className="alert alert-warning d-flex align-items-center">
          <i className="bi bi-exclamation-triangle me-2 fs-5"></i>
          <div className="flex-grow-1">
            <strong>No estás identificado.</strong>
            <br />
            <small>
              Para hacer una reserva, abrí la consola del navegador (F12) y ejecutá:{" "}
              <code style={{ background: "rgba(0,0,0,0.3)", padding: "2px 6px", borderRadius: 4 }}>
                localStorage.setItem('userId', '1')
              </code>
            </small>
          </div>
        </div>
      )}

      <div className="d-flex gap-3 mb-4 flex-wrap">
        <Legend className="available" label="Disponible" />
        <Legend className="selected" label="Seleccionado" />
        <Legend className="reserved" label="Reservado" />
        <Legend className="sold" label="Vendido" />
      </div>

      <div className="row g-4">
        <div className="col-12 col-lg-9">
          {processedData.sectors.map(sector => (
            <div key={sector.sectorId} className="card mb-4 shadow-sm">
              <div className="card-header d-flex justify-content-between align-items-center">
                <h5 className="mb-0">{sector.sectorName}</h5>
                <span className="badge bg-primary fs-6">
                  ${sector.price.toLocaleString("es-AR")}
                </span>
              </div>
              <div className="card-body seat-map-container">
                {Object.keys(sector.groupedSeats).sort().map(row => (
                  <div key={row} className="row-seats">
                    <span className="row-label">{row}</span>
                    {sector.groupedSeats[row].map(seat => {
                      const selected = selectedSeats.some(s => s.id === seat.id);
                      return (
                        <button
                          key={seat.id}
                          disabled={seat.status !== "Available" || submitting}
                          onClick={() => toggleSeat(seat, sector.price, sector.sectorName)}
                          className={`seat ${selected ? "selected" : ""} ${seat.status.toLowerCase()}`}
                          title={`${seat.rowIdentifier}${seat.seatNumber} — ${seat.status}`}
                        >
                          <MdEventSeat size={14} />
                          <span className="seat-label">
                            {seat.rowIdentifier}{seat.seatNumber}
                          </span>
                        </button>
                      );
                    })}
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>

        <div className="col-12 col-lg-3">
          <div className="card shadow-sm summary-card">
            <div className="card-header bg-primary text-white">
              <i className="bi bi-cart3 me-2"></i> Resumen
            </div>
            <div className="card-body">
              {selectedSeats.length === 0 ? (
                <p className="text-muted small mb-0">
                  No hay asientos seleccionados.
                </p>
              ) : (
                <>
                  <ul className="list-unstyled small mb-3">
                    {selectedSeats.map(seat => (
                      <li key={seat.id} className="d-flex justify-content-between mb-1">
                        <span>
                          {seat.sectorName} {seat.rowIdentifier}{seat.seatNumber}
                        </span>
                        <span className="text-muted">
                          ${seat.price.toLocaleString("es-AR")}
                        </span>
                      </li>
                    ))}
                  </ul>
                  <div className="d-flex justify-content-between fw-bold border-top pt-2">
                    <span>Total:</span>
                    <span>${totalAmount.toLocaleString("es-AR")}</span>
                  </div>
                </>
              )}
              <button
                className="btn btn-primary w-100 mt-3"
                disabled={selectedSeats.length === 0 || submitting || !userId}
                onClick={handleConfirmReservation}
              >
                {submitting ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2"></span>
                    Reservando...
                  </>
                ) : (
                  <>Continuar reserva</>
                )}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function Legend({ className, label }) {
  return (
    <div className="d-flex align-items-center gap-2">
      <div className={`seat seat-legend ${className}`}></div>
      <small>{label}</small>
    </div>
  );
}