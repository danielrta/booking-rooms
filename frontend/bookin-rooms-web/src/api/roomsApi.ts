import { http } from "./httpClient";

export interface EquipmentDto {
  id: number;
  name: string;
}

export interface RoomDto {
  id: number;
  name: string;
  capacity: number;
  location: string;
  equipments: EquipmentDto[];
}

export interface CreateRoomRequest {
  name: string;
  capacity: number;
  location: string;
  equipmentIds: number[];
}

export interface UpdateRoomRequest extends CreateRoomRequest {}

export function getRooms() {
  return http<RoomDto[]>("/api/rooms", {
    method: "GET",
    auth: true,
  });
}

export function createRoom(request: CreateRoomRequest) {
  return http<RoomDto>("/api/rooms", {
    method: "POST",
    body: JSON.stringify(request),
    auth: true,
  });
}

export function updateRoom(id: number, request: UpdateRoomRequest) {
  return http<RoomDto>(`/api/rooms/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
    auth: true,
  });
}

export function deleteRoom(id: number) {
  return http<void>(`/api/rooms/${id}`, {
    method: "DELETE",
    auth: true,
  });
}