import axios from "axios";
import { tokenStorage } from "./token";

function createApiClient(baseURL: string) {
  const instance = axios.create({
    baseURL,
    headers: {
      "Content-Type": "application/json",
    },
  });

  instance.interceptors.request.use((config) => {
    const token = tokenStorage.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  instance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error?.response?.status === 401) {
        tokenStorage.clearAll();
        window.location.href = "/login";
      }
      return Promise.reject(error);
    }
  );

  return instance;
}

export const authApi = createApiClient(import.meta.env.VITE_AUTH_API);
export const productApi = createApiClient(import.meta.env.VITE_PRODUCT_API);
export const movementApi = createApiClient(import.meta.env.VITE_MOVEMENT_API);
export const orchestratorApi = createApiClient(import.meta.env.VITE_ORCHESTRATOR_API);
export const queryApi = createApiClient(import.meta.env.VITE_QUERY_API);