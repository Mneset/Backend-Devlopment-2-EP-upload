import { API_URL } from "@/lib/constants";
import type { AppointmentResponseDto, RescheduleAppointmentRequestDto } from "@/types/appointment";

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
