export type SaleItem = {
  idProducto: number;
  cantidad: number;
};

export type CreateSaleSagaRequest = {
  items: SaleItem[];
};