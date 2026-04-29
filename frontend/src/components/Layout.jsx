import { Link } from "react-router-dom";

export default function Layout({ children }) {
  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark shadow-sm bg-primary">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/events">
            <i className="bi bi-ticket-perforated-fill me-2"></i>
            Ticketing
          </Link>
          <div className="d-flex">
            <Link className="btn btn-outline-light btn-sm" to="/events">
              <i className="bi bi-grid-fill me-1"></i> Eventos
            </Link>
          </div>
        </div>
      </nav>

      <main className="container py-4">{children}</main>
    </>
  );
}