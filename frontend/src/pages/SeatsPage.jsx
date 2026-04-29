import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { MdEventSeat } from "react-icons/md";
import { api } from "../api/client";
import "./SeatsPage.css";

export default function SeatsPage() {
  const { eventId } = useParams();
  const navigate = useNavigate();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedSeats, setSelectedSeats] = useState([]);

  useEffect(() => {
    api.get(`/api/v1/events/${eventId}/seats`)
      .then(setData)
      .catch(err => setError(err.message))
      .finally(() => setLoading(false));
  }, [eventId]);

  const toggleSeat = (seat, sectorPrice, sectorName) => {
    setSelectedSeats(prev => {
      const exists = prev.find(s => s.id === seat.id);
      if (exists) {
        return prev.filter(s => s.id !== seat.id);
      }
      return [...prev, { ...seat, price: sectorPrice, sectorName }];
    });
  };

  // Agrupación por sector + fila (lógica original de tu compa)
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

        Object.keys(grouped).forEach(row => {
          grouped[row].sort((a, b) => a.seatNumber - b.seatNumber);
        });

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

  if (error) {
    return <div className="alert alert-danger">Error: {error}</div>;
  }

  if (!processedData) return null;

  const totalAmount = selectedSeats.reduce((acc, s) => acc + s.price, 0);

  return (
    <div>
      <button
        className="btn btn-link text-decoration-none mb-3 p-0"
        onClick={() => navigate(`/events/${eventId}`)}
      >
        <i className="bi bi-arrow-left me-1"></i> Volver al evento
      </button>

      <h2 className="mb-3">{processedData.eventName}</h2>

      {/* Leyenda */}
      <div className="d-flex gap-3 mb-4 flex-wrap">
        <Legend className="available" label="Disponible" />
        <Legend className="selected" label="Seleccionado" />
        <Legend className="reserved" label="Reservado" />
        <Legend className="sold" label="Vendido" />
      </div>

      <div className="row g-4">
        {/* Columna principal: sectores */}
        <div className="col-lg-9">
          {processedData.sectors.map(sector => (
            <div key={sector.sectorId} className="card mb-4 shadow-sm">
              <div className="card-header d-flex justify-content-between align-items-center">
                <h5 className="mb-0">{sector.sectorName}</h5>
                <span className="badge bg-primary fs-6">
                  ${sector.price.toLocaleString("es-AR")}
                </span>
              </div>
              <div className="card-body">
                {Object.keys(sector.groupedSeats).sort().map(row => (
                  <div key={row} className="row-seats">
                    <span className="row-label">{row}</span>
                    {sector.groupedSeats[row].map(seat => {
                      const selected = selectedSeats.some(s => s.id === seat.id);
                      return (
                        <button
                          key={seat.id}
                          disabled={seat.status !== "Available"}
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

        {/* Columna lateral: resumen */}
        <div className="col-lg-3">
          <div className="card shadow-sm sticky-top" style={{ top: 20 }}>
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
                className="btn btn-success w-100 mt-3"
                disabled={selectedSeats.length === 0}
              >
                Continuar reserva
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