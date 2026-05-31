/** Estado vacío: ícono tenue + mensaje + acción opcional. */
export default function EmptyState({
  message,
  icon = "bi-inbox",
  action = null,
}) {
  return (
    <div className="text-center py-5">
      <i className={`bi ${icon} fs-1 text-overridden-muted d-block mb-2`}></i>
      <p className="text-overridden-muted">{message}</p>
      {action}
    </div>
  );
}