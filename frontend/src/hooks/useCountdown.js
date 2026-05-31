import { useEffect, useState } from "react";

/**
 * Cuenta regresiva en segundos. Pensado para el timer de la reserva (05:00 -> 00:00).
 * `initialSeconds` es el valor inicial (ej. 300 para 5 minutos). El hook devuelve:
 * - `secondsLeft`: segundos restantes, actualizado cada segundo.
 * - `expired`: booleano, true si el contador llegó a 0.
 */
export function useCountdown(initialSeconds = 0) {
  const [secondsLeft, setSecondsLeft] = useState(initialSeconds);

  useEffect(() => {
    setSecondsLeft(initialSeconds);
  }, [initialSeconds]);

  useEffect(() => {
    if (secondsLeft <= 0) return undefined;

    const intervalId = setInterval(() => {
      setSecondsLeft((current) => (current <= 1 ? 0 : current - 1));
    }, 1000);

    return () => clearInterval(intervalId);
  }, [secondsLeft > 0]); // eslint-disable-line react-hooks/exhaustive-deps

  return {
    secondsLeft,
    expired: secondsLeft <= 0,
    reset: setSecondsLeft,
  };
}