import { useState } from "react";
import EventsPage from "./pages/EventsPage";
import EventDetailsPage from "./pages/EventDetailsPage";
import SeatsPage from "./pages/SeatsPage";

function App() {
  const [view, setView] = useState("events");
  const [selectedEvent, setSelectedEvent] = useState(null);

  let screen;

  // LISTA DE EVENTOS
  if (view === "events") {
    screen = (
      <EventsPage
        onSelectEvent={(id) => {
          setSelectedEvent(id);
          setView("details");
        }}
      />
    );
  }

  // DETALLE DEL EVENTO
  if (view === "details") {
    screen = (
      <EventDetailsPage
        eventId={selectedEvent}
        onBack={() => setView("events")}
        onGoSeats={() => setView("seats")}
      />
    );
  }

  // SEATS
  if (view === "seats") {
    screen = (
      <SeatsPage
        eventId={selectedEvent}
        onBack={() => setView("details")}
      />
    );
  }

  return screen;
}

export default App;