import { useCallback, useState } from "react";

const EMPTY_CARD = {
  cardHolder: "",
  cardNumber: "",
  expirationDate: "",
  cvv: "",
};

/**
 * Estado del formulario de tarjeta. `setField(campo, valor)` actualiza un solo
 * campo sin pisar el resto, usando una propiedad computada: { ...prev, [field]: value }.
 */
export function useCardForm(initial = EMPTY_CARD) {
  const [card, setCard] = useState(initial);

  const setField = useCallback(
    (field, value) => setCard((prev) => ({ ...prev, [field]: value })),
    []
  );

  return { card, setField, setCard };
}