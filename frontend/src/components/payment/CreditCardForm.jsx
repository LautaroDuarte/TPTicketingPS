/**
 * Formulario de tarjeta CONTROLADO: el estado vive afuera (useCardForm) y se
 * recibe por props. `onField(campo, valor)` notifica cada cambio.
 */
export default function CreditCardForm({ card, onField }) {
  return (
    <div className="row g-3">
      <div className="col-12">
        <label className="form-label small">Número de tarjeta</label>
        <input
          type="text"
          className="form-control"
          placeholder="4111 1111 1111 1111"
          value={card.cardNumber}
          onChange={(e) => onField("cardNumber", e.target.value)}
        />
      </div>
      <div className="col-12">
        <label className="form-label small">Titular</label>
        <input
          type="text"
          className="form-control"
          placeholder="Como figura en la tarjeta"
          value={card.cardHolder}
          onChange={(e) => onField("cardHolder", e.target.value)}
        />
      </div>
      <div className="col-6">
        <label className="form-label small">Vencimiento</label>
        <input
          type="text"
          className="form-control"
          placeholder="MM/AA"
          value={card.expirationDate}
          onChange={(e) => onField("expirationDate", e.target.value)}
        />
      </div>
      <div className="col-6">
        <label className="form-label small">CVV</label>
        <input
          type="text"
          className="form-control"
          placeholder="123"
          value={card.cvv}
          onChange={(e) => onField("cvv", e.target.value)}
        />
      </div>
    </div>
  );
}