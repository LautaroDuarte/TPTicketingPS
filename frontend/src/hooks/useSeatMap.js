import { useCallback, useEffect, useMemo, useState } from "react";
import { seatsService } from "../services/seatsService";
import { groupSeatsBySector } from "../lib/seats";

/**
 * Carga y procesa el mapa de asientos de un evento.
 *
 * `refetch(silent)`:
 *  - silent = false -> muestra el spinner (carga inicial / botón Actualizar).
 *  - silent = true  -> refresca en segundo plano sin parpadeo (lo usamos tras
 *    un conflicto 409 para re-sincronizar el plano sin sacarle el contexto al
 *    usuario).
 */
export function useSeatMap(eventId) {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchSeats = useCallback(
    (silent = false) => {
      if (!silent) setLoading(true);
      setError(null);

      return seatsService
        .getByEvent(eventId)
        .then((result) => setData(result))
        .catch((err) => {
          setError(err);
          throw err;
        })
        .finally(() => {
          if (!silent) setLoading(false);
        });
    },
    [eventId]
  );

  useEffect(() => {
    fetchSeats().catch(() => {});
  }, [fetchSeats]);

  // useMemo: re-agrupamos solo cuando cambia `data`, no en cada render.
  // El agrupamiento recorre todos los asientos, así que vale la pena cachearlo.
  const processedData = useMemo(() => groupSeatsBySector(data), [data]);

  return {
    data: processedData,
    loading,
    error,
    refetch: fetchSeats,
  };
}