import React, { createContext, useContext, useState } from "react";
import { login as loginApi, type LoginRequest, type LoginResponse } from "../api/authApi";

interface AuthState {
  token: string | null;
  userName: string | null;
  roles: string[];
  isAuthenticated: boolean;
}

interface AuthContextValue extends AuthState {
  loginAsync: (request: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const AUTH_STORAGE_KEY = "booking_auth";

const getInitialAuthState = (): AuthState => {
  if (typeof window === "undefined") {
    return { token: null, userName: null, roles: [], isAuthenticated: false };
  }

  const stored = localStorage.getItem(AUTH_STORAGE_KEY);
  if (!stored) {
    return { token: null, userName: null, roles: [], isAuthenticated: false };
  }

  try {
    const parsed = JSON.parse(stored) as {
      token: string;
      userName: string;
      roles: string[];
      expiresAtUtc: string;
    };

    if (new Date(parsed.expiresAtUtc) <= new Date()) {
      localStorage.removeItem(AUTH_STORAGE_KEY);
      return { token: null, userName: null, roles: [], isAuthenticated: false };
    }

    return {
      token: parsed.token,
      userName: parsed.userName,
      roles: parsed.roles,
      isAuthenticated: true,
    };
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    return { token: null, userName: null, roles: [], isAuthenticated: false };
  }
};

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [auth, setAuth] = useState<AuthState>(() => getInitialAuthState());

  const loginAsync = async (request: LoginRequest) => {
    const response: LoginResponse = await loginApi(request);

    const newState: AuthState = {
      token: response.token,
      userName: response.userName,
      roles: response.roles,
      isAuthenticated: true,
    };

    setAuth(newState);

    localStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify({
        token: response.token,
        userName: response.userName,
        roles: response.roles,
        expiresAtUtc: response.expiresAtUtc,
      })
    );
  };

  const logout = () => {
    setAuth({ token: null, userName: null, roles: [], isAuthenticated: false });
    localStorage.removeItem(AUTH_STORAGE_KEY);
  };

  const value: AuthContextValue = {
    ...auth,
    loginAsync,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): AuthContextValue => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};