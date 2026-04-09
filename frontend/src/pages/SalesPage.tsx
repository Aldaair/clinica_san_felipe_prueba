import { useEffect, useMemo, useState } from "react";
import { createSaleSaga } from "../services/orchestrator.service";
import type { KardexRow } from "../types/kardex";
import { getKardex } from "../services/kardex.service";

const IGV_RATE = 0.18;

type SaleRow = {
  idProducto: number;
  cantidad: number;
};

export function SalesPage() {
  const [catalog, setCatalog] = useState<KardexRow[]>([]);
  const [rows, setRows] = useState<SaleRow[]>([{ idProducto: 0, cantidad: 1 }]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getKardex().then(setCatalog);
  }, []);

  function updateRow(index: number, patch: Partial<SaleRow>) {
    setRows((prev) =>
      prev.map((row, i) => (i === index ? { ...row, ...patch } : row)),
    );
  }
  function removeRow(index: number) {
    setRows((prev) => prev.filter((_, i) => i !== index));
  }

  function addRow() {
    setRows((prev) => [...prev, { idProducto: 0, cantidad: 1 }]);
  }

  const lines = rows.map((row) => {
    const product = catalog.find((p) => p.idProducto === row.idProducto);
    const precioVenta = product?.precioVenta ?? 0;
    const stock = product?.stockActual ?? 0;
    const subtotal = row.cantidad * precioVenta;
    const igv = subtotal * IGV_RATE;
    const total = subtotal + igv;

    return {
      ...row,
      nombreProducto: product?.nombreProducto ?? "",
      precioVenta,
      stock,
      subtotal,
      igv,
      total,
      invalid: row.cantidad > stock,
    };
  });

  const hasInvalidStock = lines.some((x) => x.invalid);

  const totals = useMemo(() => {
    return lines.reduce(
      (acc, item) => {
        acc.subtotal += item.subtotal;
        acc.igv += item.igv;
        acc.total += item.total;
        return acc;
      },
      { subtotal: 0, igv: 0, total: 0 },
    );
  }, [rows, catalog]);

  async function handleSave() {
    if (hasInvalidStock) {
      alert("La cantidad no debe ser mayor al stock.");
      return;
    }

    setLoading(true);
    try {
      await createSaleSaga({
        items: lines
          .filter((x) => x.idProducto > 0)
          .map((x) => ({
            idProducto: x.idProducto,
            cantidad: x.cantidad,
            //precio: x.precioVenta,
          })),
      });

      alert("Venta registrada correctamente.");
      setRows([{ idProducto: 0, cantidad: 1 }]);
    } catch (err: any) {
      alert(err?.response?.data?.message ?? "No se pudo registrar la venta.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <h2 className="text-2xl font-bold">Registrar venta</h2>
      <p className="mt-1 text-sm text-slate-500">
        Muestra precio venta y stock disponible basado en movimientos
      </p>

      <div className="mt-6 space-y-4">
        {lines.map((line, index) => (
          <div
            key={index}
            className="grid grid-cols-12 gap-4 rounded-2xl border p-4 bg-white shadow-sm"
          >
            {/* Producto */}
            <div className="col-span-4 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Producto
              </label>
              <select
                className="w-full rounded-xl border px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
                value={line.idProducto}
                onChange={(e) =>
                  updateRow(index, { idProducto: Number(e.target.value) })
                }
              >
                <option value={0}>Selecciona producto</option>
                {catalog.map((item) => (
                  <option key={item.idProducto} value={item.idProducto}>
                    {item.nombreProducto}
                  </option>
                ))}
              </select>
            </div>

            {/* Cantidad */}
            <div className="col-span-2 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Cantidad
              </label>
              <input
                type="number"
                className="w-full rounded-xl border px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
                value={line.cantidad}
                onChange={(e) =>
                  updateRow(index, { cantidad: Number(e.target.value) })
                }
              />
            </div>

            {/* Precio Venta */}
            <div className="col-span-2 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                P. Venta
              </label>
              <input
                readOnly
                className="w-full rounded-xl border bg-slate-50 px-4 py-3 text-slate-600"
                value={`S/ ${line.precioVenta.toFixed(2)}`}
              />
            </div>

            {/* Stock */}
            <div className="col-span-2 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Stock Disp.
              </label>
              <input
                readOnly
                className={`w-full rounded-xl border px-4 py-3 ${line.invalid ? "bg-red-50 text-red-700 border-red-200" : "bg-slate-50 text-slate-600"}`}
                value={line.stock}
              />
            </div>

            {/* Estado / Validación */}
            <div className="col-span-2 flex flex-col justify-between">
              <div className="text-sm text-center pt-1">
                {line.idProducto === 0 ? (
                  // Estado 1: No se ha seleccionado nada aún
                  <span className="text-slate-400 text-[10px] uppercase font-medium">
                    -
                  </span>
                ) : line.invalid ? (
                  // Estado 2: Hay producto pero no hay stock
                  <span className="font-bold text-red-600 animate-pulse text-xs uppercase">
                    Sin Stock
                  </span>
                ) : (
                  // Estado 3: Todo correcto
                  <span className="font-medium text-green-600 text-xs uppercase">
                    ✓ Disponible
                  </span>
                )}
              </div>
              {/* Botón Eliminar abajo para que coincida con la altura de los inputs */}
              <button
                type="button"
                onClick={() => removeRow(index)}
                className="h-[45px] w-full rounded-xl border border-red-100 bg-red-50 text-red-500 hover:bg-red-600 hover:text-white transition-all text-xs font-bold uppercase"
              >
                Eliminar
              </button>
            </div>

            {/* Separador sutil */}
            <div className="col-span-12 border-t border-dashed border-gray-100 my-1"></div>

            {/* Resumen de totales de la línea */}
            <div className="col-span-4 flex flex-col">
              <span className="text-[10px] text-gray-400 uppercase font-semibold">
                Subtotal
              </span>
              <span className="text-sm text-slate-600 font-medium">
                S/ {line.subtotal.toFixed(2)}
              </span>
            </div>
            <div className="col-span-4 flex flex-col border-x border-gray-50 px-4">
              <span className="text-[10px] text-gray-400 uppercase font-semibold">
                IGV (18%)
              </span>
              <span className="text-sm text-slate-600 font-medium">
                S/ {line.igv.toFixed(2)}
              </span>
            </div>
            <div className="col-span-4 flex flex-col items-end">
              <span className="text-[10px] text-gray-400 uppercase font-semibold">
                Total Línea
              </span>
              <span className="text-lg text-blue-700 font-bold">
                S/ {line.total.toFixed(2)}
              </span>
            </div>
          </div>
        ))}
      </div>

      <div className="mt-4 flex gap-3">
        <button onClick={addRow} className="rounded-xl border px-4 py-2">
          Agregar fila
        </button>
      </div>

      <div className="mt-8 rounded-2xl bg-slate-50 p-6">
        <div className="grid grid-cols-3 gap-4">
          <div>
            <p className="text-sm text-slate-500">Subtotal</p>
            <p className="text-lg font-semibold">
              S/ {totals.subtotal.toFixed(2)}
            </p>
          </div>
          <div>
            <p className="text-sm text-slate-500">IGV</p>
            <p className="text-lg font-semibold">S/ {totals.igv.toFixed(2)}</p>
          </div>
          <div>
            <p className="text-sm text-slate-500">Total</p>
            <p className="text-lg font-semibold">
              S/ {totals.total.toFixed(2)}
            </p>
          </div>
        </div>

        <button
          disabled={loading || hasInvalidStock || rows.length === 0}
          onClick={handleSave}
          className="mt-6 rounded-xl bg-slate-900 px-5 py-3 text-white disabled:opacity-50"
        >
          {loading ? "Registrando..." : "Guardar venta"}
        </button>
      </div>
    </div>
  );
}
