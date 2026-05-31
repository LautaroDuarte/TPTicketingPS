import { PAYMENT_METHODS } from "../../constants/paymentMethods";
import PaymentOption from "./PaymentOption";
import CreditCardForm from "./CreditCardForm";
import TransferNotice from "./TransferNotice";
import MercadoPagoNotice from "./MercadoPagoNotice";

/**
 * Selector de método de pago + formulario dinámico según el método elegido.
 * Junta las opciones (PaymentOption) y delega el formulario al componente
 * correspondiente. El estado de la tarjeta vive afuera y entra por props.
 */
export default function PaymentMethodSelector({
  selected,
  onSelect,
  card,
  onCardField,
}) {
  return (
    <div className="card shadow-sm mb-4">
      <div className="card-header">
        <strong>Método de pago</strong>
      </div>
      <div className="card-body">
        <div className="row g-3">
          {Object.entries(PAYMENT_METHODS).map(([value, meta]) => (
            <PaymentOption
              key={value}
              value={value}
              selected={selected}
              onSelect={onSelect}
              icon={meta.icon}
              label={meta.label}
              description={meta.description}
            />
          ))}
        </div>

        <div className="mt-4">
          {selected === "creditCard" && (
            <CreditCardForm card={card} onField={onCardField} />
          )}
          {selected === "mercadoPago" && <MercadoPagoNotice />}
          {selected === "transfer" && <TransferNotice />}
        </div>
      </div>
    </div>
  );
}