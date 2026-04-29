import { useEffect, useMemo, useState } from "react";

export default function SeatsPage({ eventId, onBack }) {
  const [data, setData] = useState(null);
  const [selectedSeats, setSelectedSeats] = useState([]);

  useEffect(() => {
    if (!eventId) return;

    fetch(`https://localhost:39716/api/v1/events/${eventId}/seats`)
      .then(res => res.json())
      .then(setData)
      .catch(err => console.error(err));
  }, [eventId]);

  const toggleSeat = (seatId) => {
    setSelectedSeats(prev =>
      prev.includes(seatId)
        ? prev.filter(id => id !== seatId)
        : [...prev, seatId]
    );
  };

  // 🔥 AGRUPACIÓN LIMPIA POR SECTOR + FILA
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

        // ordenar cada fila por número de asiento
        Object.keys(grouped).forEach(row => {
          grouped[row].sort((a, b) => a.seatNumber - b.seatNumber);
        });

        return {
          ...sector,
          groupedSeats: grouped
        };
      })
    };
  }, [data]);

  if (!processedData) return <p>Cargando asientos...</p>;

  return (
    <div>
      <button onClick={onBack}>Volver</button>

      <h2>{processedData.eventName}</h2>

      {processedData.sectors.map(sector => (
        <div key={sector.sectorId} style={{ marginBottom: 30 }}>
          <h3>
            {sector.sectorName} - ${sector.price}
          </h3>

          {/* FILAS */}
          {Object.keys(sector.groupedSeats)
            .sort()
            .map(row => (
              <div
                key={row}
                style={{
                  display: "flex",
                  gap: 5,
                  marginBottom: 5
                }}
              >
                {sector.groupedSeats[row].map(seat => {
                  const selected = selectedSeats.includes(seat.id);

                  return (
                    <button
                      key={seat.id}
                      disabled={seat.status !== "Available"}
                      onClick={() => toggleSeat(seat.id)}
                      style={{
                        width: 40,
                        height: 40,
                        background:
                          seat.status === "Available"
                            ? selected
                              ? "green"
                              : "lightgray"
                            : seat.status === "Reserved"
                            ? "orange"
                            : "red"
                      }}
                    >
                      {seat.rowIdentifier}{seat.seatNumber}
                    </button>
                  );
                })}
              </div>
            ))}
        </div>
      ))}
    </div>
  );
}