import { authApi } from "../lib/api";
import type { LoginRequest, LoginResponse } from "../types/auth";

export async function login(payload: LoginRequest) {
  const { data } = await authApi.post<LoginResponse>("/api/auth/login", payload);
  return data;
}