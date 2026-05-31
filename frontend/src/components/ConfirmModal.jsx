import Spinner from "./Spinner";

/**
 * Modal de confirmación genérico (sí/no), reutilizable para cualquier acción
 * destructiva. La Page controla la visibilidad (`open`) y provee los handlers.
 */
export default function ConfirmModal({
  open,
  title,
  children,
  confirmLabel = "Confirmar",
  cancelLabel = "Cancelar",
  confirmClassName = "btn btn-danger",
  confirmIcon = "bi-trash",
  loading = false,
  onConfirm,
  onCancel,
}) {
  if (!open) return null;

  return (
    <div
      className="modal d-block"
      tabIndex={-1}
      style={{ background: "rgba(0,0,0,0.6)" }}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div
          className="modal-content text-white"
          style={{ background: "var(--color-6)" }}
        >
          <div className="modal-header border-0">
            <h5 className="modal-title">
              <i className="bi bi-exclamation-triangle text-warning me-2"></i>
              {title}
            </h5>
            <button
              type="button"
              className="btn-close btn-close-white"
              onClick={onCancel}
              disabled={loading}
            ></button>
          </div>

          <div className="modal-body">{children}</div>

          <div className="modal-footer border-0">
            <button
              className="btn btn-outline-light"
              onClick={onCancel}
              disabled={loading}
            >
              {cancelLabel}
            </button>
            <button
              className={confirmClassName}
              onClick={onConfirm}
              disabled={loading}
            >
              {loading ? (
                <>
                  <Spinner small className="me-2" />
                  Procesando...
                </>
              ) : (
                <>
                  <i className={`bi ${confirmIcon} me-1`}></i>
                  {confirmLabel}
                </>
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}