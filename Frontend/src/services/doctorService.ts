import { API_URL } from "@/lib/constants";
import type { CreateDoctorRequestDto, DoctorResponseDto, UpdateDoctorRequestDto } from "@/types/doctor";

export async function getAllDoctors(): Promise<DoctorResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/doctors`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getAllDoctorsIncludingInactive(
  token: string,
): Promise<DoctorResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/doctors/all`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getDoctorByIdIncludingInactive(
  id: number,
  token: string,
): Promise<DoctorResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/doctors/all/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function createDoctor(
  data: CreateDoctorRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/doctors`, {
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

export async function updateDoctor(
  data: UpdateDoctorRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/doctors/${data.id}`, {
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

export async function deactivateDoctor(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/doctors/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}

export async function reactivateDoctor(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/doctors/${id}/reactivate`, {
    method: "PATCH",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}
