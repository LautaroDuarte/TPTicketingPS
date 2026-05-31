import { useCallback, useMemo, useState } from "react";

/**
 * Maneja qué asientos tiene seleccionados el usuario (estado puramente de UI,
 * sin red). Toggle: si ya está, lo saca; si no, lo agrega con precio y sector.
 */
export function useSeatSelection() {
  const [selectedSeats, setSelectedSeats] = useState([]);

  const toggleSeat = useCallback((seat, price, sectorName) => {
    // Updater funcional: recibimos el estado previo (prev) en vez de leer la
    // variable del closure. Garantiza que operamos sobre el valor más reciente.
    setSelectedSeats((prev) =>
      prev.some((s) => s.id === seat.id)
        ? prev.filter((s) => s.id !== seat.id)
        : [...prev, { ...seat, price, sectorName }]
    );
  }, []);

  const isSelected = useCallback(
    (seatId) => selectedSeats.some((s) => s.id === seatId),
    [selectedSeats]
  );

  const clear = useCallback(() => setSelectedSeats([]), []);

  const totalAmount = useMemo(
    () => selectedSeats.reduce((acc, seat) => acc + seat.price, 0),
    [selectedSeats]
  );

  return { selectedSeats, toggleSeat, isSelected, clear, totalAmount };
}