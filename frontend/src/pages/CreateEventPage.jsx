import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useEventForm } from "../hooks/useEventForm";
import { useCreateEvent } from "../hooks/useCreateEvent";
import { toast } from "../components/toast";
import { parseApiError } from "../lib/errors";
import Spinner from "../components/Spinner";
import RestrictedAccess from "../components/RestrictedAccess";
import EventInfoFields from "../components/events/EventInfoFields";
import SectorListEditor from "../components/events/SectorListEditor";

export default function CreateEventPage() {
  const navigate = useNavigate();
  const { isAdmin } = useAuth();
  const {
    form,
    sectors,
    updateField,
    updateSector,
    addSector,
    removeSector,
    buildPayload,
  } = useEventForm();
  const { createEvent, loading } = useCreateEvent();

  if (!isAdmin) {
    return (
      <RestrictedAccess
        title="Acceso restringido."
        message="Solo los administradores pueden crear eventos."
      />
    );
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const result = await createEvent(buildPayload());
      toast.success(`Evento "${form.name}" creado exitosamente.`);
      navigate(`/events/${result.id}`);
    } catch (err) {
      // Para 400 con detalles de validación los unimos con " | " para que se
      // distingan claro entre sí (el flujo de creación tiene muchos campos).
      if (err.status === 400 && err.details) {
        const messages = Object.values(err.details).flat().join(" | ");
        toast.error(messages);
      } else {
        toast.error(parseApiError(err, "Error al crear el evento."));
      }
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-lg-8">
        <div className="d-flex align-items-center mb-4">
          <i
            className="bi bi-calendar-plus fs-3 me-3"
            style={{ color: "var(--color-3)" }}
          ></i>
          <div>
            <h2 className="mb-0">Crear nuevo evento</h2>
            <small className="text-overridden-muted">
              Completá los datos del evento y sus sectores
            </small>
          </div>
        </div>

        <form onSubmit={handleSubmit}>
          <EventInfoFields form={form} onField={updateField} />

          <SectorListEditor
            sectors={sectors}
            onUpdateSector={updateSector}
            onAddSector={addSector}
            onRemoveSector={removeSector}
          />

          <div className="d-flex gap-2">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? (
                <>
                  <Spinner small className="me-2" />
                  Creando...
                </>
              ) : (
                <>
                  <i className="bi bi-check-lg me-1"></i>
                  Crear evento
                </>
              )}
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={() => navigate("/events")}
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}