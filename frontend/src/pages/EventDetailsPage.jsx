import { useEffect, useState } from "react";

export default function EventDetailsPage({
  eventId,
  onBack,
  onGoSeats
}) {
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!eventId) return;

    fetch(`https://localhost:39716/api/v1/events/${eventId}`)
      .then(res => res.json())
      .then(data => setEvent(data))
      .finally(() => setLoading(false));
  }, [eventId]);

  if (loading) return <p>Cargando...</p>;
  if (!event) return <p>No se encontró el evento</p>;

  return (
    <div>
      <h1>{event.name}</h1>

      <p>
        Fecha: {new Date(event.eventDate).toLocaleDateString()}
      </p>

      {event.description && <p>{event.description}</p>}

      <button onClick={onBack}>
        Volver
      </button>

      <button
        onClick={onGoSeats}
        style={{ marginLeft: 10 }}
      >
        Ver asientos
      </button>
    </div>
  );
}