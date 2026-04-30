import Toastify from "toastify-js";
import "toastify-js/src/toastify.css";

const baseConfig = {
  duration: 4000,
  gravity: "top",
  position: "right",
  stopOnFocus: true,
  close: true,
  style: {
    borderRadius: "8px",
    padding: "12px 20px",
    fontSize: "14px",
    boxShadow: "0 4px 12px rgba(0, 0, 0, 0.3)",
  },
};

const colors = {
  success: "#198754",
  error: "#dc3545",
  warning: "#ffc107",
  info: "#0090F8",  // tu color-3 de la paleta
};

function show(text, type) {
  const isWarning = type === "warning";
  Toastify({
    ...baseConfig,
    text,
    style: {
      ...baseConfig.style,
      background: colors[type],
      color: isWarning ? "#000" : "#fff",
    },
  }).showToast();
}

export const toast = {
  success: (text) => show(text, "success"),
  error: (text) => show(text, "error"),
  warning: (text) => show(text, "warning"),
  info: (text) => show(text, "info"),
};