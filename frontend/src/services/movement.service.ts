import { movementApi } from "../lib/api";
import type { KardexRow, ProductMovement } from "../types/kardex";

type StockResponseApi = {
  idProducto: number;
  nombreProducto: string;
  stockActual: number;
  costo: number;
  precioVenta: number;
};

type MovementHistoryItemResponseApi = {
  fechaRegistro: string;
  tipoMovimiento: string;
  cantidad: number;
};

function mapStockRow(item: StockResponseApi): KardexRow {
  return {
    idProducto: item.idProducto,
    nombreProducto: item.nombreProducto,
    stockActual: item.stockActual,
    costo: item.costo,
    precioVenta: item.precioVenta,
  };
}

function mapMovement(item: MovementHistoryItemResponseApi): ProductMovement {
  return {
    fechaRegistro: item.fechaRegistro,
    tipoMovimiento: item.tipoMovimiento,
    cantidad: item.cantidad,
  };
}


export async function getStockByProductId(productId: number) {
  const { data } = await movementApi.get<StockResponseApi>(`/api/movements/stock/${productId}`);
  return mapStockRow(data);
}

export async function getProductMovements(productId: number) {
  const { data } = await movementApi.get<MovementHistoryItemResponseApi[]>(
    `/api/movements/history/${productId}`
  );
  return data.map(mapMovement);
}