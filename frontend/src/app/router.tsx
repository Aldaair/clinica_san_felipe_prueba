import { createBrowserRouter, Navigate } from "react-router-dom";
import { ProtectedRoute } from "../components/ProtectedRoute";
import { AppShell } from "../components/AppShell";
import { LoginPage } from "../pages/LoginPage";
import { PurchasesPage } from "../pages/PurchasesPage";
import { SalesPage } from "../pages/SalesPage";
import { KardexPage } from "../pages/KardexPage";

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <AppShell />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <Navigate to="/compras" replace /> },
      { path: "compras", element: <PurchasesPage /> },
      { path: "ventas", element: <SalesPage /> },
      { path: "kardex", element: <KardexPage /> },
    ],
  },
]);