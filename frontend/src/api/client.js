const API_URL = import.meta.env.VITE_API_URL;

/**
 * Wrapper de fetch que:
 * - Prepende la URL del backend.
 * - Agrega Content-Type y X-User-Id automáticamente.
 * - Lanza errores con info útil cuando la respuesta no es OK.
 */
async function request(path, options = {}) {
  // ================================
  // MOCK AUTH (TEMPORAL)
  // Mientras no haya login/JWT,
  // usamos un userId fijo para poder
  // probar reservas.
  // ================================
  const userId = localStorage.getItem("userId") || "1";

  // ================================
  // HEADERS REQUEST
  // ================================
  const headers = {
    "Content-Type": "application/json",
    "X-User-Id": userId, // ⚠️ MOCK: se reemplaza por JWT luego
    ...options.headers,
  };

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers,
  });

  // Si la respuesta no tiene contenido (204 No Content), devolvemos null
  if (response.status === 204) {
    return null;
  }

  const data = await response.json();

  if (!response.ok) {
    // Lanzamos error con la info que devolvió el backend
    const error = new Error(data.message || "Error en la petición");
    error.status = response.status;
    error.details = data.details;
    error.payload = data;
    throw error;
  }

  return data;
}

// Métodos cómodos
export const api = {
  get: (path) => request(path, { method: "GET" }),
  post: (path, body) =>
    request(path, { method: "POST", body: JSON.stringify(body) }),
  put: (path, body) =>
    request(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: (path) => request(path, { method: "DELETE" }),
};
