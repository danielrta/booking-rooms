import { http } from "./httpClient";

export interface ReservationDto {
  id: number;
  roomId: number;
  roomName: string;
  date: string;
  startTimeUtc: string;
  endTimeUtc: string;
  userId: string;
  userName: string;
  status?: string;
}

export interface CreateReservationRequest {
  roomId: number;
  date: string;
  startTime: string;
  endTime: string;
}

export function getReservations() {
  return http<ReservationDto[]>("/api/reservations", {
    method: "GET",
    auth: true,
  });
}

export function createReservation(request: CreateReservationRequest) {
  return http<ReservationDto>("/api/reservations", {
    method: "POST",
    body: JSON.stringify(request),
    auth: true,
  });
}