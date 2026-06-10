import { useCallback, useState } from "react";

const EMPTY_SECTOR = () => ({
  name: "",
  price: "",
  rows: "",
  seatsPerRow: "",
});

const EMPTY_FORM = {
  name: "",
  eventDate: "",
  venue: "",
  description: "",
  maxReservationsPerUser: 4,
};

/**
 * Estado del formulario de creación de evento: datos del evento + array de sectores.
 *
 * Devuelve helpers granulares (updateField, updateSector, addSector, removeSector)
 * y `buildPayload`, que arma el objeto final convirtiendo strings a números
 * antes de mandarlo al backend (el <input type="number"> guarda strings).
 */
export function useEventForm() {
  const [form, setForm] = useState(EMPTY_FORM);
  const [sectors, setSectors] = useState([EMPTY_SECTOR()]);

  const updateField = useCallback(
    (field, value) => setForm((prev) => ({ ...prev, [field]: value })),
    []
  );

  const updateSector = useCallback((index, field, value) => {
    setSectors((prev) => {
      // Copia inmutable: tomamos el array previo, generamos uno nuevo
      // reemplazando SOLO el sector en `index` con el campo actualizado.
      const next = [...prev];
      next[index] = { ...next[index], [field]: value };
      return next;
    });
  }, []);

  const addSector = useCallback(
    () => setSectors((prev) => [...prev, EMPTY_SECTOR()]),
    []
  );

  const removeSector = useCallback((index) => {
    setSectors((prev) => {
      // Nunca dejamos el array vacío: siempre tiene que haber al menos un sector.
      if (prev.length === 1) return prev;
      // filter con (_, i) descarta el sector en la posición `index`.
      return prev.filter((_, i) => i !== index);
    });
  }, []);

  const buildPayload = useCallback(
    () => ({
      ...form,
      maxReservationsPerUser: parseInt(form.maxReservationsPerUser, 10) || 4,
      sectors: sectors.map((s) => ({
        name: s.name,
        price: parseFloat(s.price) || 0,
        rows: parseInt(s.rows, 10) || 1,
        seatsPerRow: parseInt(s.seatsPerRow, 10) || 1,
      })),
    }),
    [form, sectors]
  );

  return {
    form,
    sectors,
    updateField,
    updateSector,
    addSector,
    removeSector,
    buildPayload,
  };
}