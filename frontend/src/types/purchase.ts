export type PurchaseItem = {
  idProducto: number;
  cantidad: number;
  precio: number;
};

export type CreatePurchaseSagaRequest = {
  items: PurchaseItem[];
};