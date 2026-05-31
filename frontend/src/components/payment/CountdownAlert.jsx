import { formatMmSs } from "../../lib/time";

/** Alerta del timer de la reserva. Cambia a rojo cuando expira. */
export default function CountdownAlert({ secondsLeft, expired }) {
  return (
    <div
      className={`alert ${expired ? "alert-danger" : "alert-info"} d-flex align-items-center justify-content-between`}
    >
      <div>
        <i className="bi bi-clock me-2"></i>
        {expired ? (
          <strong>Tu reserva ha expirado.</strong>
        ) : (
          <>
            Tiempo restante: <strong>{formatMmSs(secondsLeft)}</strong>
          </>
        )}
      </div>
    </div>
  );
}