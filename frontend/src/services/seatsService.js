import { api } from "../api/client";

/** Endpoints del mapa de asientos de un evento. */
export const seatsService = {
  getByEvent: (eventId) => api.get(`/api/v1/events/${eventId}/seats`),
};