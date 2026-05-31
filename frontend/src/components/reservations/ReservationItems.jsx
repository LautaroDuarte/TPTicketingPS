import { formatCurrency } from "../../lib/format";

/**
 * Lista de ítems (asientos) de una reserva con su precio unitario.
 * Reutilizada en la pantalla de pago y en el comprobante.
 * `showIcon` agrega el ticket para el comprobante.
 */
export default function ReservationItems({ items, showIcon = false }) {
  return (
    <ul className="list-unstyled mb-3">
      {items.map((item) => (
        <li key={item.id} className="d-flex justify-content-between mb-1">
          <span>
            {showIcon && (
              <i
                className="bi bi-ticket-perforated me-2"
                style={{ color: "var(--color-3)" }}
              ></i>
            )}
            {item.sectorName} - Fila {item.rowIdentifier}, Asiento{" "}
            {item.seatNumber}
          </span>
          <span className="text-overridden-muted">
            {formatCurrency(item.unitPrice)}
          </span>
        </li>
      ))}
    </ul>
  );
}