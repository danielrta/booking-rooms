import { http } from "./httpClient";

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  userName: string;
  roles: string[];
}

export function login(request: LoginRequest) {
  return http<LoginResponse>("/api/auth/login", {
    method: "POST",
    body: JSON.stringify(request),
    auth: false,
  });
}