import { orchestratorApi } from "../lib/api";
import type { CreatePurchaseSagaRequest } from "../types/purchase";
import type { CreateSaleSagaRequest } from "../types/sale";

export async function createPurchaseSaga(payload: CreatePurchaseSagaRequest) {
  const { data } = await orchestratorApi.post("/api/purchasesagas", payload);
  return data;
}

export async function createSaleSaga(payload: CreateSaleSagaRequest) {
  const { data } = await orchestratorApi.post("/api/salesagas", payload);
  return data;
}