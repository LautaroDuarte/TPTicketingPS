import { useCallback, useState } from "react";
import { paymentsService } from "../services/paymentsService";

/** Procesa el pago de una reserva. `processing` deshabilita el botón mientras corre. */
export function usePayment(reservationId) {
  const [processing, setProcessing] = useState(false);

  const pay = useCallback(
    async (payload) => {
      setProcessing(true);
      try {
        return await paymentsService.pay(reservationId, payload);
      } finally {
        setProcessing(false);
      }
    },
    [reservationId]
  );

  return { pay, processing };
}