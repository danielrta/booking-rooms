import { useState } from "react";
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  MenuItem,
  Alert,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { createUser, type CreateUserRequest } from "../../api/usersApi";

interface UserFormValues {
  userName: string;
  password: string;
  role: string;
}

export function UsersPage() {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<UserFormValues>({
    defaultValues: {
      userName: "",
      password: "",
      role: "User",
    },
  });

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onSubmit = async (values: UserFormValues) => {
    setError(null);
    setSuccess(null);

    const request: CreateUserRequest = {
      userName: values.userName,
      password: values.password,
      role: values.role,
    };

    try {
      await createUser(request);
      setSuccess("User created successfully.");
      reset({ userName: "", password: "", role: "User" });
    } catch (err: any) {
      setError(
        err.message ||
          "There was an error while creating the user. Please try again."
      );
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ maxWidth: 480, mx: "auto", p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Create user
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Only admins can create new users.
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <Box
          component="form"
          onSubmit={handleSubmit(onSubmit)}
          sx={{ display: "flex", flexDirection: "column", gap: 2 }}
        >
          <TextField
            label="Email"
            fullWidth
            {...register("userName", {
                required: "Email is required",
                pattern: {
                value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: "Invalid email address",
                },
            })}
            error={!!errors.userName}
            helperText={errors.userName?.message}
            />

          <TextField
            label="Password"
            type="password"
            fullWidth
            {...register("password", {
              required: "Password is required",
              minLength: {
                value: 6,
                message: "At least 6 characters",
              },
            })}
            error={!!errors.password}
            helperText={errors.password?.message}
          />

          <TextField
            label="Role"
            select
            fullWidth
            defaultValue="User"
            {...register("role", {
              required: "Role is required",
            })}
            error={!!errors.role}
            helperText={errors.role?.message}
          >
            <MenuItem value="User">User</MenuItem>
            <MenuItem value="Admin">Admin</MenuItem>
          </TextField>

          <Box sx={{ display: "flex", justifyContent: "flex-end", mt: 1 }}>
            <Button
              type="submit"
              variant="contained"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Saving..." : "Create user"}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
}