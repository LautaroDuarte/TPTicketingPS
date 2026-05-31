/**
 * Agrupa los asientos de cada sector por fila y los ordena por número.
 */
export function groupSeatsBySector(data) {
  if (!data) return null;

  return {
    ...data,
    sectors: data.sectors.map((sector) => {
      // reduce construye un diccionario fila -> asientos.
      // El acumulador (acc) arranca como {} y se va completando.
      const grouped = sector.seats.reduce((acc, seat) => {
        const row = seat.rowIdentifier;
        if (!acc[row]) acc[row] = [];
        acc[row].push(seat);
        return acc;
      }, {});

      // Ordenamos cada fila por número de asiento (in place sobre cada array).
      Object.keys(grouped).forEach((row) =>
        grouped[row].sort((a, b) => a.seatNumber - b.seatNumber)
      );

      return { ...sector, groupedSeats: grouped };
    }),
  };
}