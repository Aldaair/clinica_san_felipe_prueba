import { useEffect, useMemo, useState } from "react";
import { ProductModal } from "../components/ProductModal";
import { getProducts } from "../services/products.service";
import { createPurchaseSaga } from "../services/orchestrator.service";
import type { Product } from "../types/product";

type PurchaseRow = {
  idProducto: number;
  cantidad: number;
  precio: number;
};

export function PurchasesPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [rows, setRows] = useState<PurchaseRow[]>([
    { idProducto: 0, cantidad: 1, precio: 0 },
  ]);
  const [loading, setLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false);

  async function loadProducts() {
    const data = await getProducts();
    setProducts(data);
  }

  useEffect(() => {
    loadProducts();
  }, []);

  function updateRow(index: number, patch: Partial<PurchaseRow>) {
    setRows((prev) =>
      prev.map((row, i) => (i === index ? { ...row, ...patch } : row)),
    );
  }

  function addRow() {
    setRows((prev) => [...prev, { idProducto: 0, cantidad: 1, precio: 0 }]);
  }

  function removeRow(index: number) {
    setRows((prev) => prev.filter((_, i) => i !== index));
  }

  const totals = useMemo(() => {
    const subtotal = rows.reduce(
      (acc, row) => acc + row.cantidad * row.precio,
      0,
    );
    const igv = subtotal * 0.18;
    const total = subtotal + igv;
    return { subtotal, igv, total };
  }, [rows]);

  async function handleSave() {
    setLoading(true);
    try {
      await createPurchaseSaga({ items: rows.filter((x) => x.idProducto > 0) });
      alert("Compra registrada correctamente.");
      setRows([{ idProducto: 0, cantidad: 1, precio: 0 }]);
    } catch (err: any) {
      alert(err?.response?.data?.message ?? "No se pudo registrar la compra.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold">Registrar compra</h2>
          <p className="text-sm text-slate-500">
            Permite varios productos y alta rápida en modal
          </p>
        </div>

        <button
          onClick={() => setModalOpen(true)}
          className="rounded-xl bg-emerald-600 px-4 py-2 text-white"
        >
          Registrar producto
        </button>
      </div>

      <div className="space-y-4">
        {rows.map((row, index) => (
          <div
            key={index}
            className="grid grid-cols-12 gap-4 rounded-2xl border p-4 bg-white shadow-sm"
          >
            {/* Grupo Producto */}
            <div className="col-span-5 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Producto
              </label>
              <select
                className="w-full rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
                value={row.idProducto}
                onChange={(e) => {
                  const productId = Number(e.target.value);
                  const product = products.find((p) => p.id === productId);
                  updateRow(index, {
                    idProducto: productId,
                    precio: product?.costo ?? 0,
                  });
                }}
              >
                <option value={0}>Selecciona producto</option>
                {products.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.nombreProducto} - {product.nroLote}
                  </option>
                ))}
              </select>
            </div>

            {/* Grupo Cantidad */}
            <div className="col-span-2 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Cantidad
              </label>
              <input
                type="number"
                className="w-full rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
                placeholder="0"
                value={row.cantidad}
                onChange={(e) =>
                  updateRow(index, { cantidad: Number(e.target.value) })
                }
              />
            </div>

            {/* Grupo Costo */}
            <div className="col-span-3 flex flex-col gap-1.5">
              <label className="text-xs font-bold text-gray-500 uppercase ml-1">
                Costo
              </label>
              <input
                type="number"
                step="0.01"
                className="w-full rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
                placeholder="0.00"
                value={row.precio}
                onChange={(e) =>
                  updateRow(index, { precio: Number(e.target.value) })
                }
              />
            </div>

            {/* Botón Eliminar */}
            <div className="col-span-2 flex flex-col justify-end">
              <button
                type="button"
                onClick={() => removeRow(index)}
                className="h-[50px] rounded-xl border border-red-200 bg-red-50 text-red-600 hover:bg-red-600 hover:text-white transition-all font-medium"
              >
                Eliminar
              </button>
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
        <div className="grid grid-cols-3 gap-4 text-sm">
          <div>
            <p className="text-slate-500">Subtotal</p>
            <p className="text-lg font-semibold">
              S/ {totals.subtotal.toFixed(2)}
            </p>
          </div>
          <div>
            <p className="text-slate-500">IGV</p>
            <p className="text-lg font-semibold">S/ {totals.igv.toFixed(2)}</p>
          </div>
          <div>
            <p className="text-slate-500">Total</p>
            <p className="text-lg font-semibold">
              S/ {totals.total.toFixed(2)}
            </p>
          </div>
        </div>

        <button
          disabled={loading || rows.length === 0}
          onClick={handleSave}
          className="mt-6 rounded-xl bg-slate-900 px-5 py-3 text-white disabled:opacity-50"
        >
          {loading ? "Registrando..." : "Guardar compra"}
        </button>
      </div>

      <ProductModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreated={loadProducts}
      />
    </div>
  );
}
