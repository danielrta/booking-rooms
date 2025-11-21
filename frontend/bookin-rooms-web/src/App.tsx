import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { LoginPage } from "./auth/LoginPage";
import { RoomsPage } from "./features/rooms/RoomsPage";
import { ReservationsPage } from "./features/reservations/ReservationsPage";
import { MainLayout } from "./layout/MainLayout";
import { UsersPage } from "./features/users/UsersPage";

const App: React.FC = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
        
          <Route path="/login" element={<LoginPage />} />

          <Route element={<ProtectedRoute />}>
              <Route element={<MainLayout />}>
              <Route path="/reservations" element={<ReservationsPage />} />

              <Route element={<ProtectedRoute roles={["Admin"]} />}>
                <Route path="/users" element={<UsersPage />} />
                <Route path="/rooms" element={<RoomsPage />} />
                <Route path="/home" element={<Navigate to="/rooms" replace />} />
              </Route>

              <Route path="/" element={<Navigate to="/" replace />} />
            </Route>
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
};

export default App;