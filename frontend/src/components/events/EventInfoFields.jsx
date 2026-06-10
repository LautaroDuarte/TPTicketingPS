/**
 * Sección "Información del evento" del formulario de creación.
 * Controlado: recibe `form` y notifica cada cambio por `onField(campo, valor)`.
 */
export default function EventInfoFields({ form, onField }) {
  return (
    <div className="card mb-4">
      <div className="card-header">
        <i className="bi bi-info-circle me-2"></i>
        Información del evento
      </div>
      <div className="card-body">
        <div className="row g-3">
          <div className="col-12">
            <label className="form-label small">Nombre del evento *</label>
            <input
              type="text"
              className="form-control"
              placeholder="Ej: Concierto de Rock 2026"
              value={form.name}
              onChange={(e) => onField("name", e.target.value)}
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label small">Fecha y hora *</label>
            <input
              type="datetime-local"
              className="form-control"
              value={form.eventDate}
              onChange={(e) => onField("eventDate", e.target.value)}
              required
            />
          </div>

          <div className="col-md-6">
            <label className="form-label small">Recinto *</label>
            <input
              type="text"
              className="form-control"
              placeholder="Ej: Estadio Luna Park"
              value={form.venue}
              onChange={(e) => onField("venue", e.target.value)}
              required
            />
          </div>

          <div className="col-12">
            <label className="form-label small">Descripción</label>
            <textarea
              className="form-control"
              rows={3}
              placeholder="Descripción opcional del evento..."
              value={form.description}
              onChange={(e) => onField("description", e.target.value)}
            />
          </div>

          <div className="col-md-4">
            <label className="form-label small">Máx. reservas por usuario</label>
            <input
              type="number"
              className="form-control"
              min={1}
              max={20}
              value={form.maxReservationsPerUser}
              onChange={(e) => onField("maxReservationsPerUser", e.target.value)}
            />
          </div>
        </div>
      </div>
    </div>
  );
}