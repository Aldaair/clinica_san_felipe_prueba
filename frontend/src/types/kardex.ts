    export type KardexRow = {
  idProducto: number;
  nombreProducto: string;
  stockActual: number;
  costo: number;
  precioVenta: number;
};

export type ProductMovement = {
  fechaRegistro: string;
  tipoMovimiento: string;
  cantidad: number;
};