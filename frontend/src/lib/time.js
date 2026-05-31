/** Segundos totales -> "MM:SS" con padding (ej. 65 -> "01:05"). */
export const formatMmSs = (totalSeconds) => {
  const minutes = Math.floor(totalSeconds / 60);
  const seconds = totalSeconds % 60;
  return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
};

/**
 * Etiqueta de tiempo restante hasta `expiresAt`, calculada contra el reloj actual.
 * No es un contador vivo: devuelve el valor para el momento en que se la llama.
 */
export const timeRemainingLabel = (expiresAt) => {
  const diffMs = new Date(expiresAt) - new Date();
  if (diffMs <= 0) return "Expirada";

  const minutes = Math.floor(diffMs / 60000);
  const seconds = Math.floor((diffMs % 60000) / 1000);
  return `${minutes}:${String(seconds).padStart(2, "0")}`;
};

/** True si la fecha de expiración ya pasó. */
export const isExpired = (expiresAt) => new Date(expiresAt) <= new Date();