/** Spinner de Bootstrap reutilizable. */
export default function Spinner({ small = false, className = "" }) {
  const sizeClass = small ? "spinner-border-sm" : "";
  return (
    <span
      className={`spinner-border ${sizeClass} ${className}`}
      role="status"
    ></span>
  );
}