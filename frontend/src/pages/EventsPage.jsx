import { useEffect, useState } from "react";

export default function EventsPage({ onSelectEvent }) {
  const [events, setEvents] = useState([]);
  const [page, setPage] = useState(1);

  useEffect(() => {
    fetch(`https://localhost:39716/api/v1/events?page=${page}&pageSize=10`)
      .then(res => res.json())
      .then(setEvents);
  }, [page]);

  return (
    <div>
      <h1>Eventos</h1>

      <ul>
        {events.map(e => (
          <li key={e.id}>
            {e.name} - {new Date(e.eventDate).toLocaleDateString()}

            <button
              onClick={() => onSelectEvent(e.id)}
              style={{ marginLeft: 10 }}
            >
              Ver evento
            </button>
          </li>
        ))}
      </ul>

      <button onClick={() => setPage(p => Math.max(p - 1, 1))}>
        Anterior
      </button>

      <button onClick={() => setPage(p => p + 1)}>
        Siguiente
      </button>
    </div>
  );
}