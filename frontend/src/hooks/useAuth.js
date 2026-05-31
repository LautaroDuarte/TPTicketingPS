import { useCallback, useState } from "react";
import { authService } from "../services/authService";

const SESSION_KEYS = {
  id: "userId",
  name: "userName",
  role: "userRole",
  email: "userEmail",
};

/** Lee la sesión persistida en localStorage como un objeto plano. */
function readSession() {
  return {
    id: localStorage.getItem(SESSION_KEYS.id),
    name: localStorage.getItem(SESSION_KEYS.name),
    role: localStorage.getItem(SESSION_KEYS.role),
    email: localStorage.getItem(SESSION_KEYS.email),
  };
}

/**
 * Maneja la sesión del usuario (login/logout) y la persiste en localStorage.
 *
 * useState(readSession): le pasamos la FUNCIÓN, no readSession(). React solo la
 * ejecuta en el primer render (lazy initializer); evita leer localStorage en
 * cada render, que sería trabajo inútil.
 */
export function useAuth() {
  const [user, setUser] = useState(readSession);
  const [loading, setLoading] = useState(false);

  const login = useCallback(async (credentials) => {
    setLoading(true);
    try {
      const loggedUser = await authService.login(credentials);
      localStorage.setItem(SESSION_KEYS.id, loggedUser.id);
      localStorage.setItem(SESSION_KEYS.name, loggedUser.name);
      localStorage.setItem(SESSION_KEYS.role, loggedUser.role);
      localStorage.setItem(SESSION_KEYS.email, loggedUser.email);
      setUser(readSession());
      return loggedUser;
    } finally {
      setLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    Object.values(SESSION_KEYS).forEach((key) => localStorage.removeItem(key));
    setUser(readSession());
  }, []);

  return {
    user,
    isAuthenticated: Boolean(user.id),
    loading,
    login,
    logout,
  };
}