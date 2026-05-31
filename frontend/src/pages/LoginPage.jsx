import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { toast } from "../components/toast";
import Spinner from "../components/Spinner";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, loading } = useAuth();

  // email/password son estado de formulario (UI), quedan bien en la Page.
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const user = await login({ email, password });
      toast.success(`Bienvenido, ${user.name}`);
      navigate("/events");
    } catch (err) {
      toast.error(err.message || "Error al iniciar sesión");
    }
  };

  return (
    <div className="row justify-content-center" style={{ marginTop: "10vh" }}>
      <div className="col-12 col-sm-8 col-md-5 col-lg-4">
        <div className="card shadow-sm">
          <div className="card-body p-4">
            <div className="text-center mb-4">
              <i
                className="bi bi-ticket-perforated-fill"
                style={{ fontSize: 48, color: "var(--color-3)" }}
              ></i>
              <h3 className="mt-2">Ticketing</h3>
              <p className="text-overridden-muted small">
                Iniciá sesión para continuar
              </p>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label small">Email</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="tu@email.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>

              <div className="mb-3">
                <label className="form-label small">Contraseña</label>
                <input
                  type="password"
                  className="form-control"
                  placeholder="Tu contraseña"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <Spinner small className="me-2" />
                    Ingresando...
                  </>
                ) : (
                  <>
                    <i className="bi bi-box-arrow-in-right me-2"></i>
                    Iniciar sesión
                  </>
                )}
              </button>
            </form>

            <div
              className="mt-4 p-3"
              style={{
                background: "rgba(255,255,255,0.05)",
                borderRadius: 8,
                fontSize: 13,
              }}
            >
              <p className="mb-1">
                <strong>Usuarios de prueba:</strong>
              </p>
              <p className="mb-1">
                <code>demo@ticketing.local</code> / <code>demo123</code>{" "}
                (usuario)
              </p>
              <p className="mb-0">
                <code>admin@ticketing.local</code> / <code>admin123</code>{" "}
                (admin)
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}