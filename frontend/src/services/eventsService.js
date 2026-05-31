import { api } from "../api/client";

 // Endpoints del catálogo de eventos.
export const eventsService = {
  list: ({ page, pageSize, status = "Active" }) =>
    api.get(`/api/v1/events?page=${page}&pageSize=${pageSize}&status=${status}`),

  getById: (eventId) => api.get(`/api/v1/events/${eventId}`),
};