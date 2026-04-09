import { useState } from "react";
import { createProduct } from "../services/products.service";

type Props = {
  open: boolean;
  onClose: () => void;
  onCreated: () => void;
};

export function ProductModal({ open, onClose, onCreated }: Props) {
  const [form, setForm] = useState({
    nombreProducto: "",
    nroLote: "",
    costo: 0,
    precioVenta: 0,
  });
  const [loading, setLoading] = useState(false);

  if (!open) return null;

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    try {
      await createProduct(form);
      onCreated();
      onClose();
      setForm({ nombreProducto: "", nroLote: "", costo: 0, precioVenta: 0 });
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 px-4">
      <form
        onSubmit={handleSave}
        className="w-full max-w-lg rounded-2xl bg-white p-6 shadow-lg"
      >
        <h3 className="text-xl font-bold text-slate-800">Registrar producto</h3>

        <div className="mt-6 grid grid-cols-2 gap-4">
          {/* Nombre del Producto */}
          <div className="col-span-2 flex flex-col gap-1.5">
            <label className="text-xs font-bold text-gray-500 uppercase ml-1">
              Nombre del Producto
            </label>
            <input
              className="rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
              placeholder="Ej. Laptop Dell Latitude"
              value={form.nombreProducto}
              onChange={(e) =>
                setForm((p) => ({ ...p, nombreProducto: e.target.value }))
              }
            />
          </div>

          {/* Lote */}
          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-bold text-gray-500 uppercase ml-1">
              Nro. Lote
            </label>
            <input
              className="rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
              placeholder="L-1024"
              value={form.nroLote}
              onChange={(e) =>
                setForm((p) => ({ ...p, nroLote: e.target.value }))
              }
            />
          </div>

          {/* Costo */}
          <div className="flex flex-col gap-1.5">
            <label className="text-xs font-bold text-gray-500 uppercase ml-1">
              Costo (Compra)
            </label>
            <input
              type="number"
              step="0.01"
              className="rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none"
              placeholder="0.00"
              value={form.costo}
              onChange={(e) =>
                setForm((p) => ({ ...p, costo: Number(e.target.value) }))
              }
            />
          </div>

          {/* Precio Venta */}
          <div className="col-span-2 flex flex-col gap-1.5">
            <label className="text-xs font-bold text-gray-500 uppercase ml-1">
              Precio de Venta
            </label>
            <input
              type="number"
              step="0.01"
              className="rounded-xl border border-gray-200 px-4 py-3 focus:ring-2 focus:ring-blue-500 outline-none font-semibold text-blue-600"
              placeholder="0.00"
              value={form.precioVenta}
              onChange={(e) =>
                setForm((p) => ({ ...p, precioVenta: Number(e.target.value) }))
              }
            />
          </div>
        </div>

        <div className="mt-8 flex justify-end gap-3">
          <button
            type="button"
            onClick={onClose}
            className="rounded-xl border border-gray-300 px-6 py-2.5 text-sm font-medium hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
          <button
            disabled={loading}
            className="rounded-xl bg-slate-900 px-6 py-2.5 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-50 transition-all"
          >
            {loading ? "Guardando..." : "Guardar Producto"}
          </button>
        </div>
      </form>
    </div>
  );
}
