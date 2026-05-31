import { useCallback, useState } from "react";
import { reservationsService } from "../services/reservationsService";

/**
 * Crea una reserva a partir de un evento y una lista de asientos.
 * Devuelve la reserva creada (la Page decide navegar/avisar) y expone
 * `submitting` para deshabilitar el botón mientras la petición está en vuelo.
 *
 * No atrapa el error a propósito: el mapeo 409/404/400 -> mensaje es decisión
 * de UX y vive en la Page. Acá solo orquestamos la llamada y el estado.
 */
export function useCreateReservation() {
  const [submitting, setSubmitting] = useState(false);

  const createReservation = useCallback(async ({ eventId, seatIds }) => {
    // TEST
    console.log(eventId, seatIds);
    setSubmitting(true);
    try {
      return await reservationsService.create({
        eventId: parseInt(eventId, 10),
        seatIds,
      });
    } finally {
      setSubmitting(false);
    }
  }, []);

  return { createReservation, submitting };
}