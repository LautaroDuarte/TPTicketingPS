import { useEffect, useState } from "react";
import { reservationsService } from "../services/reservationsService";

/** Lista las reservas de un usuario. No hace nada si todavía no hay userId. */
export function useMyReservations(userId) {
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!userId) return undefined;

    let cancelled = false;
    setLoading(true);
    setError(null);

    reservationsService
      .listByUser(userId)
      .then((data) => {
        if (!cancelled) setReservations(data);
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
  }, [userId]);

  return { reservations, loading, error };
}