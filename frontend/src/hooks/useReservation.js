import { useEffect, useState } from "react";
import { reservationsService } from "../services/reservationsService";

/**
 * Carga una reserva por id.
 *
 * `initial`: si ya recibimos la reserva por el state del router (ej. venimos de
 * la pantalla de pago), arrancamos con ella y NO disparamos el fetch. Solo
 * pegamos a la API si entraron directo por URL sin ese contexto.
 */
export function useReservation(reservationId, { initial = null } = {}) {
  const [reservation, setReservation] = useState(initial);
  const [loading, setLoading] = useState(!initial);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (initial) return undefined;

    let cancelled = false;
    setLoading(true);
    setError(null);

    reservationsService
      .getById(reservationId)
      .then((data) => {
        if (!cancelled) setReservation(data);
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
  }, [reservationId, initial]);

  return { reservation, loading, error };
}