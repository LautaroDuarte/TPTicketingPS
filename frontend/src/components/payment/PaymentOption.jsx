/** Tarjeta seleccionable de un método de pago. */
export default function PaymentOption({
  value,
  selected,
  onSelect,
  icon,
  label,
  description,
}) {
  const isSelected = value === selected;
  return (
    <div className="col-12 col-md-4">
      <div
        className={`card h-100 ${isSelected ? "border-primary" : ""}`}
        onClick={() => onSelect(value)}
        style={{ cursor: "pointer", borderWidth: 2 }}
      >
        <div className="card-body text-center">
          <i
            className={`bi ${icon} fs-1 mb-2`}
            style={{ color: isSelected ? "var(--color-3)" : "var(--text-muted)" }}
          ></i>
          <h6 className="mb-1">{label}</h6>
          <small className="text-overridden-muted">{description}</small>
        </div>
      </div>
    </div>
  );
}