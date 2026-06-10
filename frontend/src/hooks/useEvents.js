import { useCallback, useEffect, useState } from "react";
import { eventsService } from "../services/eventsService";

/**
 * Catálogo paginado de eventos. Expone los datos, el estado de carga/error
 * y los controles de paginación. La Page solo decide cómo mostrarlo.
 */
export function useEvents(initialPage = 1, pageSize = 5) {
  const [page, setPage] = useState(initialPage);
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Flag de cancelación: si la página cambia (o el componente se desmonta)
    // antes de que resuelva el fetch, ignoramos la respuesta vieja. Evita
    // "race conditions" y el warning de setState sobre un componente desmontado.
    let cancelled = false;

    setLoading(true);
    setError(null);

    eventsService
      .list({ page, pageSize })
      .then((result) => {
        if (!cancelled) setData(result);
      })
      .catch((err) => {
        if (!cancelled) setError(err);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [page, pageSize]);

  const nextPage = useCallback(() => setPage((p) => p + 1), []);
  const prevPage = useCallback(() => setPage((p) => p - 1), []);

  return {
    data,
    events: data?.items ?? [],
    page,
    setPage,
    nextPage,
    prevPage,
    loading,
    error,
  };
}