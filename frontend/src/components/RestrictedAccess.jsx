/**
 * Mensaje de acceso restringido. Reusable para cualquier pantalla con control
 * de rol (no solo admin): la Page le pasa el detalle.
 */
export default function RestrictedAccess({
  title = "Acceso restringido.",
  message = "No tenés permisos para ver esta sección.",
  icon = "bi-shield-lock",
}) {
  return (
    <div className="alert alert-danger text-center mt-5">
      <i className={`bi ${icon} fs-1 d-block mb-2`}></i>
      <strong>{title}</strong>
      <p className="mb-0 small">{message}</p>
    </div>
  );
}