import { http } from "./httpClient";

export interface CreateUserRequest {
  userName: string;
  password: string;
  role: string;
}

export interface UserResponse {
  id: string;
  userName: string;
  role: string;
}

export function createUser(request: CreateUserRequest) {
  return http("/api/auth/register", {
    method: "POST",
    body: JSON.stringify(request),
  });
}