import React, { useEffect, useState } from "react";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  TextField,
} from "@mui/material";
import { useForm, Controller } from "react-hook-form";
import {
  createReservation,
  type CreateReservationRequest,
  type ReservationDto,
} from "../../api/reservationsApi";
import { getRooms, type RoomDto } from "../../api/roomsApi";

interface ReservationFormDialogProps {
  open: boolean;
  onClose: () => void;
  onCreated: (reservation: ReservationDto) => void;
}

interface ReservationFormValues {
  roomId: number;
  date: string;
  startTime: string;
  endTime: string;
}

export const ReservationFormDialog: React.FC<ReservationFormDialogProps> = ({
  open,
  onClose,
  onCreated,
}) => {
  const [rooms, setRooms] = useState<RoomDto[]>([]);
  const [loadingRooms, setLoadingRooms] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    control,
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<ReservationFormValues>({
    defaultValues: {
      roomId: 0,
      date: "",
      startTime: "",
      endTime: "",
    },
  });

  const startTime = watch("startTime");

  useEffect(() => {
    if (!open) return;
    setError(null);
    setLoadingRooms(true);

    getRooms()
      .then(setRooms)
      .catch(() => {
        setError("Error loading rooms.");
      })
      .finally(() => setLoadingRooms(false));
  }, [open]);

  const handleClose = () => {
    if (saving) return;
    reset({
      roomId: 0,
      date: "",
      startTime: "",
      endTime: "",
    });
    setError(null);
    onClose();
  };

  const toTimeWithSeconds = (value: string) => {
    if (!value) return value;
    if (value.length === 5) {
      return `${value}:00`;
    }
    return value;
  };

  const onSubmit = async (values: ReservationFormValues) => {
    setSaving(true);
    setError(null);

    const request: CreateReservationRequest = {
      roomId: values.roomId,
      date: values.date,
      startTime: toTimeWithSeconds(values.startTime),
      endTime: toTimeWithSeconds(values.endTime),
    };

    try {
      const created = await createReservation(request);
      onCreated(created);
      reset({
        roomId: 0,
        date: "",
        startTime: "",
        endTime: "",
      });
      onClose();
    } catch (err: any) {
      setError(
        err.body?.title ||
          err.body?.detail ||
          err.body?.message ||
          err.message ||
          "Error creating reservation."
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>New reservation</DialogTitle>
      <DialogContent>
        <Box sx={{ mt: 1, display: "flex", flexDirection: "column", gap: 2 }}>
          <FormControl fullWidth error={!!errors.roomId} disabled={loadingRooms}>
            <InputLabel id="room-label">Room</InputLabel>
            <Controller
              name="roomId"
              control={control}
              rules={{ required: "Room is required" }}
              render={({ field }) => (
                <Select
                  labelId="room-label"
                  label="Room"
                  value={field.value || ""}
                  onChange={field.onChange}
                >
                  {rooms.map((room) => (
                    <MenuItem key={room.id} value={room.id}>
                      {room.name}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
            <FormHelperText>
              {errors.roomId ? errors.roomId.message : ""}
            </FormHelperText>
          </FormControl>

          <TextField
            label="Date"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            {...register("date", {
              required: "Date is required",
            })}
            error={!!errors.date}
            helperText={errors.date?.message || ""}
          />

          <Box sx={{ display: "flex", gap: 2 }}>
            <TextField
              label="Start time"
              type="time"
              fullWidth
              InputLabelProps={{ shrink: true }}
              inputProps={{ step: 60 }}
              {...register("startTime", {
                required: "Start time is required",
              })}
              error={!!errors.startTime}
              helperText={errors.startTime?.message || ""}
            />

            <TextField
              label="End time"
              type="time"
              fullWidth
              InputLabelProps={{ shrink: true }}
              inputProps={{ step: 60 }}
              {...register("endTime", {
                required: "End time is required",
                validate: (value) => {
                  if (!startTime || !value) return true;
                  if (value <= startTime) {
                    return "End time must be after start time";
                  }
                  return true;
                },
              })}
              error={!!errors.endTime}
              helperText={errors.endTime?.message || ""}
            />
          </Box>

          {error && (
            <Box sx={{ color: "error.main", fontSize: 14 }}>{error}</Box>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} disabled={saving}>
          Cancel
        </Button>
        <Button onClick={handleSubmit(onSubmit)} variant="contained" disabled={saving}>
          {saving ? "Saving..." : "Save"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};