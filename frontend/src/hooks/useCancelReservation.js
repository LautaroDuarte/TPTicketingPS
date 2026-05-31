import { useCallback, useState } from "react";
import { reservationsService } from "../services/reservationsService";

/** Cancela una reserva (libera los asientos en el backend). */
export function useCancelReservation(reservationId) {
  const [cancelling, setCancelling] = useState(false);

  const cancel = useCallback(async () => {
    setCancelling(true);
    try {
      return await reservationsService.cancel(reservationId);
    } finally {
      setCancelling(false);
    }
  }, [reservationId]);

  return { cancel, cancelling };
}