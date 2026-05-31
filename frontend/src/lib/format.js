/**
 * Formateo de moneda en pesos argentinos.
 * Number(value ?? 0) cubre null/undefined sin reventar con .toLocaleString.
 */
export const formatCurrency = (value) =>
  `$${Number(value ?? 0).toLocaleString("es-AR")}`;

/** Formateo de fecha/hora local. `options` es opcional (Intl.DateTimeFormat). */
export const formatDateTime = (value, options) =>
  new Date(value).toLocaleString("es-AR", options);