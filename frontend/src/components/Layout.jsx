import { Link, useNavigate } from "react-router-dom";

export default function Layout({ children }) {
  const navigate = useNavigate();
  const userName = localStorage.getItem("userName");
  const userRole = localStorage.getItem("userRole");
  const isLoggedIn = !!localStorage.getItem("userId");

  const handleLogout = () => {
    localStorage.removeItem("userId");
    localStorage.removeItem("userName");
    localStorage.removeItem("userRole");
    localStorage.removeItem("userEmail");
    navigate("/login");
  };

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark shadow-sm bg-primary">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/events">
            <i className="bi bi-ticket-perforated-fill me-2"></i>
            Ticketing
          </Link>

          <div className="d-flex align-items-center gap-2">
            <Link className="btn btn-outline-light btn-sm" to="/events">
              <i className="bi bi-grid-fill me-1"></i> Eventos
            </Link>

            {isLoggedIn ? (
              <div className="dropdown">
                <button
                  className="btn btn-outline-light btn-sm dropdown-toggle"
                  type="button"
                  data-bs-toggle="dropdown"
                >
                  <i className="bi bi-person-circle me-1"></i>
                  {userName}
                  {userRole === "admin" && (
                    <span className="badge bg-warning text-dark ms-1" style={{ fontSize: 9 }}>
                      ADMIN
                    </span>
                  )}
                </button>
                <ul className="dropdown-menu dropdown-menu-end">
                  <li>
                    <Link className="dropdown-item" to="/my-reservations">
                      <i className="bi bi-ticket me-2"></i>Mis reservas
                    </Link>
                  </li>
                  <li><hr className="dropdown-divider" /></li>
                  <li>
                    <button className="dropdown-item text-danger" onClick={handleLogout}>
                      <i className="bi bi-box-arrow-right me-2"></i>Cerrar sesión
                    </button>
                  </li>
                </ul>
              </div>
            ) : (
              <Link className="btn btn-outline-light btn-sm" to="/login">
                <i className="bi bi-box-arrow-in-right me-1"></i> Ingresar
              </Link>
            )}
          </div>
        </div>
      </nav>

      <main className="container py-4">{children}</main>
    </>
  );
}