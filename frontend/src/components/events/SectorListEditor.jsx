import SectorEditor from "./SectorEditor";

/**
 * Card con la lista completa de sectores + botón "Agregar sector".
 * Orquesta los SectorEditor: cada uno notifica hacia arriba por índice.
 */
export default function SectorListEditor({
  sectors,
  onUpdateSector,
  onAddSector,
  onRemoveSector,
}) {
  return (
    <div className="card mb-4">
      <div className="card-header d-flex justify-content-between align-items-center">
        <span>
          <i className="bi bi-grid-3x3 me-2"></i>
          Sectores ({sectors.length})
        </span>
        <button
          type="button"
          className="btn btn-outline-primary btn-sm"
          onClick={onAddSector}
        >
          <i className="bi bi-plus-lg me-1"></i>
          Agregar sector
        </button>
      </div>

      <div className="card-body">
        {sectors.map((sector, index) => (
          // Nota: usar el índice como key es aceptable acá porque los sectores
          // se identifican por posición y nunca se reordenan. Si en el futuro
          // se permitiera reordenarlos, habría que asignarles un id estable.
          <SectorEditor
            key={index}
            sector={sector}
            index={index}
            canRemove={sectors.length > 1}
            onField={(field, value) => onUpdateSector(index, field, value)}
            onRemove={() => onRemoveSector(index)}
          />
        ))}
      </div>
    </div>
  );
}