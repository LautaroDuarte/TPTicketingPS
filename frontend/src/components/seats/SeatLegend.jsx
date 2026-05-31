/** Referencia de colores del mapa (Disponible / Seleccionado / etc.). */
export default function SeatLegend({ className, label }) {
  return (
    <div className="d-flex align-items-center gap-2">
      <div className={`seat seat-legend ${className}`}></div>
      <small>{label}</small>
    </div>
  );
}