import { getStatusConfig } from "../../constants/reservationStatus";

/** Badge de estado de una reserva (color + ícono + texto según el estado). */
export default function StatusBadge({ status }) {
  const config = getStatusConfig(status);
  return (
    <span className={`badge ${config.className}`}>
      <i className={`bi ${config.icon} me-1`}></i>
      {config.label}
    </span>
  );
}