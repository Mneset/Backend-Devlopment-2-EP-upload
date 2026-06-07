import { API_URL } from "@/lib/constants";
import type { ClinicResponseDto, CreateClinicRequestDto, UpdateClinicRequestDto } from "@/types/clinic";

export async function getAllClinics(): Promise<ClinicResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/clinics`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getAllClinicsIncludingInactive(
  token: string,
): Promise<ClinicResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/clinics/all`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getClinicByIdIncludingInactive(
  id: number,
  token: string,
): Promise<ClinicResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/clinics/all/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function createClinic(
  data: CreateClinicRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/clinics`, {
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

export async function updateClinic(
  data: UpdateClinicRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/clinics/${data.id}`, {
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

export async function deactivateClinic(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/clinics/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}

export async function reactivateClinic(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/clinics/${id}/reactivate`, {
    method: "PATCH",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}
