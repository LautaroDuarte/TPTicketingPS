import { useCallback, useState } from "react";
import { eventsService } from "../services/eventsService";

/** Crea un evento. La Page se encarga del payload, los toasts y la navegación. */
export function useCreateEvent() {
  const [loading, setLoading] = useState(false);

  const createEvent = useCallback(async (payload) => {
    setLoading(true);
    try {
      return await eventsService.create(payload);
    } finally {
      setLoading(false);
    }
  }, []);

  return { createEvent, loading };
}