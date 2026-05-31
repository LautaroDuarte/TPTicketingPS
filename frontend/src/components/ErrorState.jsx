/**
 * Estado de error genérico: ícono + mensaje + acciones.
 * `actions` es el slot donde la Page mete los botones que correspondan
 * (Volver, Reintentar, etc.). Mantiene el componente agnóstico de la pantalla.
 */
export default function ErrorState({
  message = "Ocurrió un error.",
  icon = "bi-exclamation-circle",
  actions = null,
}) {
  return (
    <div className="text-center py-5">
      <i className={`bi ${icon} fs-1 text-warning`}></i>
      <p className="mt-3 mb-3">{message}</p>
      {actions && (
        <div className="d-flex gap-2 justify-content-center flex-wrap">
          {actions}
        </div>
      )}
    </div>
  );
}