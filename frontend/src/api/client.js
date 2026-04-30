const API_URL = import.meta.env.VITE_API_URL || "https://localhost:39716";

/**
 * Wrapper de fetch que:
 * - Prepende la URL del backend.
 * - Agrega Content-Type y X-User-Id automáticamente.
 * - Lanza errores con info útil cuando la respuesta no es OK.
 */
async function request(path, options = {}) {
  const userId = localStorage.getItem("userId");

  const headers = {
    "Content-Type": "application/json",
    ...options.headers,
  };

  // Mientras no haya auth real, mandamos el userId desde localStorage
  if (userId) {
    headers["X-User-Id"] = userId;
  }

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers,
  });

  // Si la respuesta no tiene contenido (204 No Content), devolvemos null
  if (response.status === 204) return null;

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    const error = new Error(data?.message || "Error en la petición");
    error.status = response.status;
    error.details = data?.details;
    error.payload = data;
    throw error;
  }

  return data;
}

export const api = {
  get: (path) => request(path, { method: "GET" }),
  post: (path, body) => request(path, { method: "POST", body: JSON.stringify(body) }),
  put: (path, body) => request(path, { method: "PUT", body: JSON.stringify(body) }),
  delete: (path) => request(path, { method: "DELETE" }),
};