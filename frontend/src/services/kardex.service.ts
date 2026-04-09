import { queryApi } from "../lib/api";
import type { KardexRow, ProductMovement } from "../types/kardex";

export async function getKardex() {
  const { data } = await queryApi.get<KardexRow[]>("/api/kardex");
  return data;
}

export async function getProductMovements(productId: number) {
  const { data } = await queryApi.get<ProductMovement[]>(`/api/kardex/${productId}/movements`);
  return data;
}