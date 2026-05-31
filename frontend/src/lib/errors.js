/**
 * Traduce un error del cliente HTTP a un mensaje legible para el usuario.
 * Si el error tiene un campo "details" con mensajes específicos, los concatena.
 * Si no, devuelve el mensaje general del error o un mensaje de fallback.
 */
export function parseApiError(err, fallback = "Ocurrió un error.") {
  if (err?.details) {
    const messages = Object.values(err.details).flat().join(" ");
    if (messages) return messages;
  }
  return err?.message || fallback;
}