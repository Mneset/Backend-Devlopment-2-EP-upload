import { API_URL } from "@/lib/constants";
import type { AppointmentResponseDto, BookAppointmentRequestDto, BookGuestAppointmentRequestDto, RescheduleAppointmentRequestDto } from "@/types/appointment";

export async function bookGuestAppointment(
  data: BookGuestAppointmentRequestDto,
): Promise<{ success: true; data: AppointmentResponseDto } | { success: false; status: number }> {
  const response = await fetch(`${API_URL}/api/v1/appointments/guest`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    return { success: false, status: response.status };
  }

  return { success: true, data: await response.json() };
}

export type BookAppointmentResult =
  | { success: true; data: AppointmentResponseDto }
  | { success: false; status: number };

export async function bookAppointment(
  data: BookAppointmentRequestDto,
  token: string,
): Promise<BookAppointmentResult> {
  const response = await fetch(`${API_URL}/api/v1/appointments`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    return { success: false, status: response.status };
  }

  return { success: true, data: await response.json() };
}

export async function getMyAppointments(
  token: string,
): Promise<AppointmentResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/appointments/mine`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getAppointmentById(
  id: number,
  token: string,
): Promise<AppointmentResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/appointments/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function cancelAppointment(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/appointments/${id}/cancel`, {
    method: "PATCH",
    headers: { Authorization: `Bearer ${token}` },
  });

  return response.ok;
}

export async function rescheduleAppointment(
  data: RescheduleAppointmentRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/appointments/${data.id}/reschedule`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  return response.ok;
}
