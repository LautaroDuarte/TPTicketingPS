import { Routes, Route, Navigate, Link } from "react-router-dom";
import EventsPage from "./pages/EventsPage";
import EventDetailsPage from "./pages/EventDetailsPage";
import SeatsPage from "./pages/SeatsPage";
import PaymentPage from "./pages/PaymentPage";
import ConfirmationPage from "./pages/ConfirmationPage";
import Layout from "./components/Layout";

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Navigate to="/events" replace />} />
        <Route path="/events" element={<EventsPage />} />
        <Route path="/events/:eventId" element={<EventDetailsPage />} />
        <Route path="/events/:eventId/seats" element={<SeatsPage />} />
        <Route path="/reservations/:reservationId/payment" element={<PaymentPage />} />
        <Route path="/reservations/:reservationId/confirmation" element={<ConfirmationPage />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Layout>
  );
}

function NotFound() {
  return (
    <div className="text-center py-5">
      <h1 className="display-1">404</h1>
      <p className="lead">Página no encontrada</p>
      <Link className="btn btn-primary" to="/events">
        Volver al inicio
      </Link>
    </div>
  );
}

export default App;