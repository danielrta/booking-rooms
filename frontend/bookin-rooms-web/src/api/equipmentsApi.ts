import { http } from "./httpClient";

export interface EquipmentDto {
  id: number;
  name: string;
}

export function getEquipments() {
  return http<EquipmentDto[]>("/api/equipments", {
    method: "GET",
    auth: true,
  });
}