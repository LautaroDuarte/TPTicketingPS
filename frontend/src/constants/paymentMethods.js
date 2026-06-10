/** Métodos de pago disponibles y su presentación. */
export const PAYMENT_METHODS = {
  creditCard: {
    icon: "bi-credit-card-2-front",
    label: "Tarjeta de crédito",
    description: "Visa, Mastercard, Amex",
  },
  mercadoPago: {
    icon: "bi-wallet2",
    label: "Mercado Pago",
    description: "Saldo o tarjetas asociadas",
  },
  transfer: {
    icon: "bi-bank",
    label: "Transferencia",
    description: "CBU / Alias",
  },
};

export const getPaymentMethod = (key) =>
  PAYMENT_METHODS[key] || PAYMENT_METHODS.creditCard;

/**
 * Datos por defecto para Mercado Pago y Transferencia: el backend espera campos de tarjeta,
 * así que para MP y Transferencia los completamos al construir el payload (sin tocar el form).
 */
export const MERCADO_PAGO_DEFAULTS = {
  cardHolder: "MercadoPago User",
  cardNumber: "4111111111111111",
  expirationDate: "12/26",
  cvv: "000",
};
export const TRANSFER_DEFAULTS = {
  cardHolder: "MercadoPago User",
  cardNumber: "4111111111111111",
  expirationDate: "12/26",
  cvv: "000",
};