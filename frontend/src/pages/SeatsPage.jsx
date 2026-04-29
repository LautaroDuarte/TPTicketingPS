import { useEffect, useMemo, useState } from "react";
import { MdEventSeat } from "react-icons/md";
import "./SeatsPage.css";
export default function SeatsPage({ eventId, onBack }) {
  const [data, setData] = useState(null);
  const [selectedSeats, setSelectedSeats] = useState([]);

  useEffect(() => {
    if (!eventId) return;

    fetch(`https://localhost:39716/api/v1/events/${eventId}/seats`)
      .then((res) => res.json())
      .then(setData)
      .catch((err) => console.error(err));
  }, [eventId]);

  const toggleSeat = (seat) => {
    setSelectedSeats((prev) => {
      const exists = prev.find((s) => s.id === seat.id);

      if (exists) {
        return prev.filter((s) => s.id !== seat.id);
      }

      return [...prev, seat];
    });
  };

  // 🔥 AGRUPACIÓN LIMPIA POR SECTOR + FILA
  const processedData = useMemo(() => {
    if (!data) return null;

    return {
      ...data,
      sectors: data.sectors.map((sector) => {
        const grouped = sector.seats.reduce((acc, seat) => {
          const row = seat.rowIdentifier;

          if (!acc[row]) acc[row] = [];

          acc[row].push(seat);
          return acc;
        }, {});

        // ordenar cada fila por número de asiento
        Object.keys(grouped).forEach((row) => {
          grouped[row].sort((a, b) => a.seatNumber - b.seatNumber);
        });

        return {
          ...sector,
          groupedSeats: grouped,
        };
      }),
    };
  }, [data]);

  if (!processedData) return <p>Cargando asientos...</p>;

  return (
    <div className="seats-page">
      <div className="seats-content">
        <button onClick={onBack}>Volver</button>

        <h2>{processedData.eventName}</h2>

        <div className="sectors">
          {processedData.sectors.map((sector) => (
            <div key={sector.sectorId} className="sector">
              <h3>
                {sector.sectorName} - ${sector.price}
              </h3>

              {Object.keys(sector.groupedSeats)
                .sort()
                .map((row) => (
                  <div key={row} className="row">
                    {sector.groupedSeats[row].map((seat) => {
                      const selected = selectedSeats.some(
                        (s) => s.id === seat.id,
                      );

                      return (
                        <button
                          key={seat.id}
                          disabled={seat.status !== "Available"}
                          onClick={() => toggleSeat(seat)}
                          className={`seat ${selected ? "selected" : ""} ${seat.status.toLowerCase()}`}
                        >
                          <MdEventSeat size={16} />
                          {seat.rowIdentifier}
                          {seat.seatNumber}
                        </button>
                      );
                    })}
                  </div>
                ))}
            </div>
          ))}
        </div>
      </div>

      <div className="summary">
        <h3>Resumen</h3>

        {selectedSeats.length === 0 && <p>No hay asientos seleccionados</p>}

        {selectedSeats.map((seat) => (
          <div key={seat.id} className="summary-item">
            {seat.sectorName} {seat.rowIdentifier}
            {seat.seatNumber} - ${seat.price}
          </div>
        ))}

        <hr />

        <div className="total">
          Total: ${selectedSeats.reduce((acc, s) => acc + s.price, 0)}
        </div>

        <button disabled={selectedSeats.length === 0}>Continuar reserva</button>
      </div>
    </div>
  );
}
