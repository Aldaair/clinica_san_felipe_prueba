import { Link, Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const navItems = [
  { to: "/compras", label: "Compras" },
  { to: "/ventas", label: "Ventas" },
  { to: "/kardex", label: "Kardex" },
];

export function AppShell() {
  const { pathname } = useLocation();
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-slate-100">
      <header className="border-b bg-white">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">
          <div>
            <h1 className="text-xl font-bold">San Felipe Front</h1>
            <p className="text-sm text-slate-500">Compras, Ventas y Kardex</p>
          </div>

          <div className="flex items-center gap-4">
            <div className="text-right text-sm">
              <p className="font-medium">{user?.fullName}</p>
              <p className="text-slate-500">{user?.role}</p>
            </div>
            <button
              onClick={() => {
                logout();
                navigate("/login");
              }}
              className="rounded-lg bg-slate-900 px-4 py-2 text-white"
            >
              Salir
            </button>
          </div>
        </div>
      </header>

      <div className="mx-auto grid max-w-7xl grid-cols-[220px_1fr] gap-6 px-6 py-6">
        <aside className="rounded-2xl bg-white p-4 shadow-sm">
          <nav className="space-y-2">
            {navItems.map((item) => {
              const active = pathname === item.to;
              return (
                <Link
                  key={item.to}
                  to={item.to}
                  className={`block rounded-xl px-4 py-3 text-sm font-medium ${
                    active ? "bg-slate-900 text-white" : "bg-slate-50 text-slate-700 hover:bg-slate-100"
                  }`}
                >
                  {item.label}
                </Link>
              );
            })}
          </nav>
        </aside>

        <main className="rounded-2xl bg-white p-6 shadow-sm">
          <Outlet />
        </main>
      </div>
    </div>
  );
}