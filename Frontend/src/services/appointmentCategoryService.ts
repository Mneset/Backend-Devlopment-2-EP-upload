import { API_URL } from "@/lib/constants";
import type { AppointmentCategoryResponseDto, CreateAppointmentCategoryRequestDto, UpdateAppointmentCategoryRequestDto } from "@/types/appointmentCategory";

export async function getAllAppointmentCategories(): Promise<
  AppointmentCategoryResponseDto[] | null
> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getAppointmentCategoryById(
  id: number,
): Promise<AppointmentCategoryResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories/${id}`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function createAppointmentCategory(
  data: CreateAppointmentCategoryRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    return false;
  }

  return true;
}

export async function updateAppointmentCategory(
  data: UpdateAppointmentCategoryRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories/${data.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    return false;
  }

  return true;
}

export async function deleteAppointmentCategory(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}
