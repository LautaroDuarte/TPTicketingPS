import { useEffect, useState } from "react";

function App() {
  const [events, setEvents] = useState([]);
  const [page, setPage] = useState(1);

  useEffect(() => {
    fetch(`https://localhost:39716/api/V1/events?page=${page}&pageSize=10`)
      .then(res => res.json())
      .then(data => {
        console.log("DATA:", data);
        setEvents(data);
      })
      .catch(err => console.error(err));
  }, [page]);

  return (
    <div>
      <h1>Eventos</h1>

      <ul>
        {events.map(e => (
          <li key={e.id}>
            {e.name} - {new Date(e.eventDate).toLocaleDateString()}
          </li>
        ))}
      </ul>

      <div style={{ marginTop: 20 }}>
        <button onClick={() => setPage(p => Math.max(p - 1, 1))}>
          Anterior
        </button>

        <span style={{ margin: "0 10px" }}>
          Página {page}
        </span>

        <button onClick={() => setPage(p => p + 1)}>
          Siguiente
        </button>
      </div>
    </div>
  );
}

export default App;