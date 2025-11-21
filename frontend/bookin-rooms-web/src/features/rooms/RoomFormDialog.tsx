import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  Chip,
  FormHelperText,
} from "@mui/material";
import { useForm, Controller } from "react-hook-form";
import {
  createRoom,
  updateRoom,
  type RoomDto,
  type CreateRoomRequest,
  type UpdateRoomRequest,
} from "../../api/roomsApi";
import { getEquipments, type EquipmentDto } from "../../api/equipmentsApi";

interface RoomFormDialogProps {
  open: boolean;
  initialRoom?: RoomDto | null;
  onClose: () => void;
  onSaved: (room: RoomDto) => void;
}

interface RoomFormValues {
  name: string;
  capacity: number;
  location: string;
  equipmentIds: number[];
}

export const RoomFormDialog: React.FC<RoomFormDialogProps> = ({
  open,
  initialRoom,
  onClose,
  onSaved,
}) => {
  const [equipmentOptions, setEquipmentOptions] = useState<EquipmentDto[]>([]);
  const [loadingEquipments, setLoadingEquipments] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEdit = !!initialRoom;

  const {
    register,
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<RoomFormValues>({
    defaultValues: {
      name: "",
      capacity: 0,
      location: "",
      equipmentIds: [],
    },
  });

  useEffect(() => {
    if (!open) return;
    setError(null);
    setLoadingEquipments(true);
    getEquipments()
      .then(setEquipmentOptions)
      .catch(() => {
        setError("Error loading equipments.");
      })
      .finally(() => setLoadingEquipments(false));
  }, [open]);

  useEffect(() => {
    if (open && initialRoom) {
      reset({
        name: initialRoom.name,
        capacity: initialRoom.capacity,
        location: initialRoom.location,
        equipmentIds: initialRoom.equipments?.map((e) => e.id) ?? [],
      });
    }
    if (open && !initialRoom) {
      reset({
        name: "",
        capacity: 0,
        location: "",
        equipmentIds: [],
      });
    }
  }, [open, initialRoom, reset]);

  const handleClose = () => {
    if (saving) return;
    reset({
      name: "",
      capacity: 0,
      location: "",
      equipmentIds: [],
    });
    setError(null);
    onClose();
  };

  const onSubmit = async (values: RoomFormValues) => {
    setSaving(true);
    setError(null);
  
    const request: CreateRoomRequest | UpdateRoomRequest = {
      name: values.name.trim(),
      capacity: Number(values.capacity),
      location: values.location.trim(),
      equipmentIds: values.equipmentIds,
    };
  
    try {
      let savedRoom: RoomDto;
  
      if (isEdit && initialRoom) {
        await updateRoom(initialRoom.id, request as UpdateRoomRequest);
  
        const updatedEquipments =
          equipmentOptions.filter((e) =>
            request.equipmentIds.includes(e.id)
          ) ?? [];
  
        savedRoom = {
          ...initialRoom,
          name: request.name,
          capacity: request.capacity,
          location: request.location,
          equipments: updatedEquipments,
        };
      } else {
        const created = await createRoom(request as CreateRoomRequest);
        savedRoom = created;
      }
  
      onSaved(savedRoom);
  
      reset({
        name: "",
        capacity: 0,
        location: "",
        equipmentIds: [],
      });
      onClose();
    } catch (err: any) {
      setError(
        err.body?.title ||
          err.body?.detail ||
          err.body?.message ||
          err.message ||
          "Error saving room."
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>{isEdit ? "Edit room" : "Create room"}</DialogTitle>
      <DialogContent>
        <Box sx={{ mt: 1, display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Name"
            fullWidth
            {...register("name", {
              required: "Name is required",
              minLength: { value: 3, message: "Min 3 characters" },
            })}
            error={!!errors.name}
            helperText={errors.name?.message || ""}
          />

          <TextField
            label="Capacity"
            type="number"
            fullWidth
            {...register("capacity", {
              required: "Capacity is required",
              valueAsNumber: true,
              min: { value: 1, message: "Capacity must be greater than 0" },
            })}
            error={!!errors.capacity}
            helperText={errors.capacity?.message || ""}
          />

          <TextField
            label="Location"
            fullWidth
            {...register("location", {
              required: "Location is required",
              minLength: { value: 2, message: "Min 2 characters" },
            })}
            error={!!errors.location}
            helperText={errors.location?.message || ""}
          />

          <FormControl fullWidth disabled={loadingEquipments}>
            <InputLabel id="equipments-label">Equipments</InputLabel>
            <Controller
              name="equipmentIds"
              control={control}
              render={({ field }) => (
                <Select
                  labelId="equipments-label"
                  multiple
                  value={field.value}
                  onChange={field.onChange}
                  input={<OutlinedInput label="Equipments" />}
                  renderValue={(selected) => (
                    <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                      {selected.map((id) => {
                        const eq = equipmentOptions.find((x) => x.id === id);
                        return (
                          <Chip key={id} label={eq?.name ?? id} size="small" />
                        );
                      })}
                    </Box>
                  )}
                >
                  {equipmentOptions.map((eq) => (
                    <MenuItem key={eq.id} value={eq.id}>
                      {eq.name}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
            <FormHelperText>
              Optional. You can select multiple equipments.
            </FormHelperText>
          </FormControl>

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
          {saving ? "Saving..." : isEdit ? "Update" : "Save"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};