import { useEffect, useState } from "react";
import { getProductMovements } from "../services/movement.service";
import type { KardexRow, ProductMovement } from "../types/kardex";
import { getKardex } from "../services/kardex.service";

export function KardexPage() {
  const [rows, setRows] = useState<KardexRow[]>([]);
  const [movements, setMovements] = useState<ProductMovement[]>([]);
  const [selected, setSelected] = useState<KardexRow | null>(null);

  useEffect(() => {
    getKardex().then(setRows);
  }, []);

  async function openMovements(row: KardexRow) {
    setSelected(row);
    const data = await getProductMovements(row.idProducto);
    setMovements(data);
  }

  return (
    <div>
      <h2 className="text-2xl font-bold">Kardex</h2>
      <p className="mt-1 text-sm text-slate-500">
        Stock actual, costo y precio venta por producto
      </p>

      <div className="mt-6 overflow-hidden rounded-2xl border">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-50 text-left">
            <tr>
              <th className="px-4 py-3">Id</th>
              <th className="px-4 py-3">Producto</th>
              <th className="px-4 py-3">Stock</th>
              <th className="px-4 py-3">Costo</th>
              <th className="px-4 py-3">Precio venta</th>
              <th className="px-4 py-3">Acción</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((row) => (
              <tr key={row.idProducto} className="border-t">
                <td className="px-4 py-3">{row.idProducto}</td>
                <td className="px-4 py-3">{row.nombreProducto}</td>
                <td className="px-4 py-3">{row.stockActual}</td>
                <td className="px-4 py-3">S/ {row.costo.toFixed(2)}</td>
                <td className="px-4 py-3">S/ {row.precioVenta.toFixed(2)}</td>
                <td className="px-4 py-3">
                  <button
                    onClick={() => openMovements(row)}
                    className="rounded-lg bg-slate-900 px-3 py-2 text-white"
                  >
                    Ver movimientos
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selected && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
          <div className="w-full max-w-2xl rounded-2xl bg-white p-6">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-bold">Movimientos del producto</h3>
                <p className="text-sm text-slate-500">{selected.nombreProducto}</p>
              </div>
              <button onClick={() => setSelected(null)} className="rounded-lg border px-3 py-2">
                Cerrar
              </button>
            </div>

            <div className="mt-4 overflow-hidden rounded-2xl border">
              <table className="min-w-full text-sm">
                <thead className="bg-slate-50 text-left">
                  <tr>
                    <th className="px-4 py-3">Fecha</th>
                    <th className="px-4 py-3">Tipo</th>
                    <th className="px-4 py-3">Cantidad</th>
                  </tr>
                </thead>
                <tbody>
                  {movements.map((movement, index) => (
                    <tr key={index} className="border-t">
                      <td className="px-4 py-3">{movement.fechaRegistro}</td>
                      <td className="px-4 py-3">{movement.tipoMovimiento}</td>
                      <td className="px-4 py-3">{movement.cantidad}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}