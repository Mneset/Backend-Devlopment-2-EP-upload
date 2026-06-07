import { API_URL } from "@/lib/constants";
import type { MyProfileResponseDto, PatientResponseDto, RegisterPatientRequestDto, UpdateOwnProfileRequestDto, UpdatePatientRequestDto } from "@/types/patient";

export async function register(
  data: RegisterPatientRequestDto,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/auth/register/patient`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  return response.ok;
}

export async function getMyProfile(
  token: string,
): Promise<MyProfileResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/patients/me`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function updateOwnProfile(
  data: UpdateOwnProfileRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/patients/me`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  return response.ok;
}

export async function deleteOwnProfile(
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/patients/me`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  return response.ok;
}

export async function getAllPatientsIncludingInactive(
  token: string,
): Promise<PatientResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/patients/all`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getPatientByIdIncludingInactive(
  id: number,
  token: string,
): Promise<PatientResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/patients/all/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function updatePatient(
  data: UpdatePatientRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/patients/${data.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });

  return response.ok;
}

export async function deactivatePatient(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/patients/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  return response.ok;
}

export async function reactivatePatient(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/patients/${id}/reactivate`, {
    method: "PATCH",
    headers: { Authorization: `Bearer ${token}` },
  });

  return response.ok;
}
