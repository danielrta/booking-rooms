import React from "react";
import { Button, Container, Typography } from "@mui/material";
import { useAuth } from "../../auth/AuthContext";

export const HomePage: React.FC = () => {
  const { userName, logout, roles } = useAuth();

  return (
    <Container sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        Meeting Rooms Booking
      </Typography>

      <Typography variant="body1" gutterBottom>
        Welcome, <strong>{userName}</strong>
      </Typography>

      <Typography variant="body2" gutterBottom>
        Roles: {roles.join(", ") || "none"}
      </Typography>

      <Button variant="outlined" sx={{ mt: 2 }} onClick={logout}>
        Logout
      </Button>
    </Container>
  );
};