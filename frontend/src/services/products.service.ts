import { productApi } from "../lib/api";
import type { Product } from "../types/product";

export async function getProducts() {
  const { data } = await productApi.get<Product[]>("/api/products");
  return data;
}

export async function createProduct(payload: {
  nombreProducto: string;
  nroLote: string;
  costo: number;
  precioVenta: number;
}) {
  const { data } = await productApi.post<Product>("/api/products", payload);
  return data;
}