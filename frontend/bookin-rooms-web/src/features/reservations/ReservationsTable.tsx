import React from "react";
import {
  Box,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import type { ReservationDto } from "../../api/reservationsApi";

interface ReservationsTableProps {
  reservations: ReservationDto[];
}

export const ReservationsTable: React.FC<ReservationsTableProps> = ({
  reservations,
}) => {
  const formatDate = (value: string) => {
    const d = new Date(value);
    if (Number.isNaN(d.getTime())) return value;

    const day = String(d.getDate()).padStart(2, "0");
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const year = d.getFullYear();

    return `${day}/${month}/${year}`;
  };

  const formatTime = (value: string) => {
    const d = new Date(value);
    if (!Number.isNaN(d.getTime())) {
      return d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    }
    return value;
  };

  return (
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Room</TableCell>
            <TableCell>Date</TableCell>
            <TableCell>Time</TableCell>
            <TableCell>User</TableCell>
            <TableCell>Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {reservations.map((r) => (
            <TableRow key={r.id} hover>
              <TableCell>{r.roomName}</TableCell>
              <TableCell>{formatDate(r.startTimeUtc)}</TableCell>
              <TableCell>
                {formatTime(r.startTimeUtc)} - {formatTime(r.endTimeUtc)}
              </TableCell>
              <TableCell>{r.userName}</TableCell>
              <TableCell>
                {r.status ? (
                  <Chip label={r.status} size="small" variant="outlined" />
                ) : (
                  <Chip
                    label="Active"
                    size="small"
                    variant="outlined"
                    color="success"
                  />
                )}
              </TableCell>
            </TableRow>
          ))}
          {reservations.length === 0 && (
            <TableRow>
              <TableCell colSpan={5}>
                <Box sx={{ py: 2 }}>
                  <Typography variant="body2" color="text.secondary">
                    No reservations found.
                  </Typography>
                </Box>
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};