import { Link } from "react-router-dom";

export default function Layout({ children }) {
  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/events">
            <i className="bi bi-ticket-perforated-fill me-2"></i>
            Ticketing
          </Link>
          <div className="d-flex">
            <Link className="btn btn-outline-light" to="/events">
              Eventos
            </Link>
          </div>
        </div>
      </nav>

      <main className="container py-4">{children}</main>
    </>
  );
}