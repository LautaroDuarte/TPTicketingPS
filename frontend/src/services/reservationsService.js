import { api } from "../api/client";

/** Ciclo de vida de una reserva: crear, consultar, cancelar y listar por usuario. */
export const reservationsService = {
  create: ({ eventId, seatIds }) =>
    api.post("/api/v1/reservations", { eventId, seatIds }),

  getById: (reservationId) => api.get(`/api/v1/reservations/${reservationId}`),

  cancel: (reservationId) => api.delete(`/api/v1/reservations/${reservationId}`),

  listByUser: (userId) => api.get(`/api/v1/users/${userId}/reservations`),
};