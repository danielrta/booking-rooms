import React, { useEffect, useState } from "react";
import {
  Alert,
  Box,
  CircularProgress,
  Divider,
  Paper,
  Toolbar,
  Typography,
  Button,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import {
  getReservations,
  type ReservationDto,
} from "../../api/reservationsApi";
import { ReservationFormDialog } from "./ReservationFormDialog";
import { ReservationsTable } from "./ReservationsTable";

export const ReservationsPage: React.FC = () => {
  const [reservations, setReservations] = useState<ReservationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [openDialog, setOpenDialog] = useState(false);

  useEffect(() => {
    let isMounted = true;

    const loadReservations = async () => {
      setLoading(true);
      setError(null);

      try {
        const data = await getReservations();
        if (isMounted) {
          setReservations(data);
        }
      } catch (err: any) {
        console.error(err);
        if (isMounted) {
          setError(
            err.body?.title ||
              err.body?.detail ||
              err.message ||
              "Error loading reservations."
          );
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    loadReservations();

    return () => {
      isMounted = false;
    };
  }, []);

  const handleNewReservation = () => {
    setOpenDialog(true);
  };

  const handleReservationCreated = (reservation: ReservationDto) => {
    setReservations((prev) => [...prev, reservation]);
    setOpenDialog(false);
  };

  return (
    <Box sx={{ width: "100%" }}>
      <Paper
        elevation={3}
        sx={{
          width: "100%",
          overflow: "hidden",
        }}
      >
        <Toolbar
          sx={{
            display: "flex",
            justifyContent: "space-between",
            gap: 2,
          }}
        >
          <Box>
            <Typography variant="h6">Reservations</Typography>
            <Typography variant="body2" color="text.secondary">
              View and manage meeting room reservations.
            </Typography>
          </Box>

          <Button
            variant="contained"
            startIcon={<AddIcon />}
            size="small"
            onClick={handleNewReservation}
          >
            New reservation
          </Button>
        </Toolbar>

        <Divider />

        {loading && (
          <Box sx={{ p: 4, display: "flex", justifyContent: "center" }}>
            <CircularProgress />
          </Box>
        )}

        {error && !loading && (
          <Box sx={{ p: 3 }}>
            <Alert severity="error">{error}</Alert>
          </Box>
        )}

        {!loading && !error && (
          <ReservationsTable reservations={reservations} />
        )}
      </Paper>

      <ReservationFormDialog
        open={openDialog}
        onClose={() => setOpenDialog(false)}
        onCreated={handleReservationCreated}
      />
    </Box>
  );
};