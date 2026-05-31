import { api } from "../api/client";

/** Autenticación. Por ahora solo login con email/password. */
export const authService = {
  login: ({ email, password }) =>
    api.post("/api/v1/users/login", { email, password }),
};