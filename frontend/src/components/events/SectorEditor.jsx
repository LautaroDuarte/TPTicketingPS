/**
 * Editor de un solo sector dentro del formulario.
 * Notifica los cambios por `onField(campo, valor)` y el borrado por `onRemove`.
 * `canRemove` controla si se muestra el botón de eliminar (la Page decide:
 * no se puede borrar el último sector).
 */
export default function SectorEditor({
  sector,
  index,
  canRemove,
  onField,
  onRemove,
}) {
  // Calculamos el total acá: es derivado del estado, no estado nuevo.
  const rows = parseInt(sector.rows, 10) || 0;
  const seatsPerRow = parseInt(sector.seatsPerRow, 10) || 0;
  const totalSeats = rows * seatsPerRow;

  return (
    <div
      className="border rounded p-3 mb-3"
      style={{ background: "rgba(255,255,255,0.03)" }}
    >
      <div className="d-flex justify-content-between align-items-center mb-2">
        <strong className="small">Sector {index + 1}</strong>
        {canRemove && (
          <button
            type="button"
            className="btn btn-outline-danger btn-sm"
            onClick={onRemove}
          >
            <i className="bi bi-trash"></i>
          </button>
        )}
      </div>

      <div className="row g-2">
        <div className="col-md-3">
          <input
            type="text"
            className="form-control form-control-sm"
            placeholder="Nombre (ej: VIP)"
            value={sector.name}
            onChange={(e) => onField("name", e.target.value)}
            required
          />
        </div>

        <div className="col-md-3">
          <div className="input-group input-group-sm">
            <span className="input-group-text">$</span>
            <input
              type="number"
              className="form-control"
              placeholder="Precio"
              min={1}
              step="0.01"
              value={sector.price}
              onChange={(e) => onField("price", e.target.value)}
              required
            />
          </div>
        </div>

        <div className="col-md-3">
          <input
            type="number"
            className="form-control form-control-sm"
            placeholder="Filas"
            min={1}
            max={30}
            value={sector.rows}
            onChange={(e) => onField("rows", e.target.value)}
            required
          />
        </div>

        <div className="col-md-3">
          <input
            type="number"
            className="form-control form-control-sm"
            placeholder="Asientos/fila"
            min={1}
            max={50}
            value={sector.seatsPerRow}
            onChange={(e) => onField("seatsPerRow", e.target.value)}
            required
          />
        </div>
      </div>

      {totalSeats > 0 && (
        <small className="text-muted mt-1 d-block">
          Total: {totalSeats} butacas
        </small>
      )}
    </div>
  );
}