import { useCallback, useEffect, useState } from "react";
import { eventsService } from "../services/eventsService";

/**
 * Carga el detalle de un evento. Devuelve `refetch` para reintentar
 * manualmente (lo usa el botón "Reintentar" de la Page).
 */
export function useEvent(eventId) {
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchEvent = useCallback(() => {
    setLoading(true);
    setError(null);

    return eventsService
      .getById(eventId)
      .then((data) => setEvent(data))
      .catch((err) => {
        setError(err);
        // Re-lanzamos para que quien llame a refetch() pueda encadenar lógica.
        throw err;
      })
      .finally(() => setLoading(false));
  }, [eventId]);

  useEffect(() => {
    // .catch vacío: el error ya quedó en el estado `error`; acá solo evitamos
    // un "unhandled rejection" en consola.
    fetchEvent().catch(() => {});
  }, [fetchEvent]);

  return { event, loading, error, refetch: fetchEvent };
}