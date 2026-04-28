import { useEffect, useState } from "react";

function App() {
  const [events, setEvents] = useState([]);

  useEffect(() => {
    fetch("https://localhost:39716/api/V1/events")
      .then(res => res.json())
      .then(data => {
        console.log("DATA:", data);
        setEvents(data);
      })
      .catch(err => console.error(err));
  }, []);

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
    </div>
  );
}

export default App;