import { createContext, useContext, useMemo, useState } from "react";
import { tokenStorage } from "../lib/token";
import type { AuthUser, LoginRequest } from "../types/auth";
import { login as loginRequest } from "../services/auth.service";

type AuthContextType = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  login: (payload: LoginRequest) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(tokenStorage.getUser());

  const value = useMemo<AuthContextType>(() => ({
    user,
    isAuthenticated: !!tokenStorage.getToken(),
    login: async (payload) => {
      const response = await loginRequest(payload);

      tokenStorage.setToken(response.accessToken);
      tokenStorage.setUser({
        username: response.username,
        fullName: response.fullName,
        role: response.role,
      });

      setUser({
        username: response.username,
        fullName: response.fullName,
        role: response.role,
      });
    },
    logout: () => {
      tokenStorage.clearAll();
      setUser(null);
    },
  }), [user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return ctx;
}