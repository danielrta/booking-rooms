import React, { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Divider,
  Paper,
  Toolbar,
  Typography,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import { getRooms, deleteRoom, type RoomDto } from "../../api/roomsApi";
import { RoomsTable } from "./RoomsTable";
import { RoomFormDialog } from "./RoomFormDialog";

export const RoomsPage: React.FC = () => {
  const [rooms, setRooms] = useState<RoomDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [openDialog, setOpenDialog] = useState(false);
  const [selectedRoom, setSelectedRoom] = useState<RoomDto | null>(null);

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [roomToDelete, setRoomToDelete] = useState<RoomDto | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    let isMounted = true;

    const loadRooms = async () => {
      setLoading(true);
      setError(null);

      try {
        const data = await getRooms();
        if (isMounted) setRooms(data);
      } catch (err: any) {
        console.error(err);
        if (isMounted) {
          setError(
            err.body?.title ||
              err.body?.detail ||
              err.message ||
              "Error loading rooms."
          );
        }
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    loadRooms();

    return () => {
      isMounted = false;
    };
  }, []);

  const handleAddRoom = () => {
    setSelectedRoom(null);
    setOpenDialog(true);
  };

  const handleEditRoom = (room: RoomDto) => {
    setSelectedRoom(room);
    setOpenDialog(true);
  };

  const handleRoomSaved = async () => {
    const data = await getRooms();
    setRooms(data);
  };

  const handleAskDeleteRoom = (roomId: number) => {
    const room = rooms.find((r) => r.id === roomId) ?? null;
    setRoomToDelete(room);
    setDeleteError(null);
    setDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!roomToDelete) return;
  
    setDeleting(true);
    setDeleteError(null);
  
    try {
      await deleteRoom(roomToDelete.id);
  
      const data = await getRooms();
      setRooms(data);
  
      setDeleteDialogOpen(false);
      setRoomToDelete(null);
    } catch (err: any) {
      setDeleteError(
        err.body?.title ||
          err.body?.detail ||
          err.body?.message ||
          err.message ||
          "Error deleting room."
      );
    } finally {
      setDeleting(false);
    }
  };

  const handleCloseDeleteDialog = () => {
    if (deleting) return;
    setDeleteDialogOpen(false);
    setRoomToDelete(null);
    setDeleteError(null);
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
            <Typography variant="h6">Rooms list</Typography>
            <Typography variant="body2" color="text.secondary">
              Manage available rooms for reservations.
            </Typography>
          </Box>

          <Button
            variant="contained"
            startIcon={<AddIcon />}
            size="small"
            onClick={handleAddRoom}
          >
            Add room
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
          <RoomsTable
            rooms={rooms}
            onDeleteRoom={handleAskDeleteRoom}
            onEditRoom={handleEditRoom}
          />
        )}
      </Paper>

      <RoomFormDialog
        open={openDialog}
        initialRoom={selectedRoom}
        onClose={() => setOpenDialog(false)}
        onSaved={handleRoomSaved}
      />

      <Dialog
        open={deleteDialogOpen}
        onClose={handleCloseDeleteDialog}
        fullWidth
        maxWidth="xs"
      >
        <DialogTitle>Delete room</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {roomToDelete
              ? `Are you sure you want to delete the room "${roomToDelete.name}"?`
              : "Are you sure you want to delete this room?"}
          </DialogContentText>
          {deleteError && (
            <Box sx={{ mt: 2, color: "error.main", fontSize: 14 }}>
              {deleteError}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog} disabled={deleting}>
            Cancel
          </Button>
          <Button
            onClick={handleConfirmDelete}
            color="error"
            variant="contained"
            disabled={deleting}
          >
            {deleting ? "Deleting..." : "Delete"}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};