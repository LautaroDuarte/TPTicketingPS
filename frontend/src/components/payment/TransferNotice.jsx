/** Datos informativos para pago por transferencia (sin inputs). */
export default function TransferNotice() {
  return (
    <div className="alert alert-info mb-0">
      <p className="mb-2">
        <strong>Datos para transferencia:</strong>
      </p>
      <p className="mb-1 small">CBU: 0000003100012345678901</p>
      <p className="mb-1 small">Alias: TICKETING.RESERVA</p>
      <p className="mb-0 small">
        Una vez confirmada, tu reserva será procesada.
      </p>
    </div>
  );
}