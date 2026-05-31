/**
 * Mapa de estado de reserva -> presentación (texto, clases de Bootstrap, ícono).
 * Centralizado acá para que cualquier componente lo consuma igual.
 */
export const RESERVATION_STATUS = {
  Pending: { label: "Pendiente", className: "bg-warning text-dark", icon: "bi-clock" },
  Paid: { label: "Pagada", className: "bg-success", icon: "bi-check-circle" },
  Expired: { label: "Expirada", className: "bg-secondary", icon: "bi-x-circle" },
  Cancelled: { label: "Cancelada", className: "bg-danger", icon: "bi-slash-circle" },
};

export const getStatusConfig = (status) =>
  RESERVATION_STATUS[status] || RESERVATION_STATUS.Pending;