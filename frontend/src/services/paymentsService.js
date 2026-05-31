import { api } from "../api/client";

 // Pago de una reserva. 
export const paymentsService = {
  pay: (reservationId, payload) =>
    api.post(`/api/v1/reservations/${reservationId}/payments`, payload),
};