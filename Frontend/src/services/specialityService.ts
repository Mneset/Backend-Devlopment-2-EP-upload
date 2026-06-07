import { API_URL } from "@/lib/constants";
import type { CreateSpecialityRequestDto, SpecialityResponseDto, UpdateSpecialityRequestDto } from "@/types/speciality";

export async function getAllSpecialities(): Promise<SpecialityResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/specialities`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function getSpecialityById(
  id: number,
): Promise<SpecialityResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/specialities/${id}`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}

export async function createSpeciality(
  data: CreateSpecialityRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/specialities`, {
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

export async function updateSpeciality(
  data: UpdateSpecialityRequestDto,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/specialities/${data.id}`, {
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

export async function deleteSpeciality(
  id: number,
  token: string,
): Promise<boolean> {
  const response = await fetch(`${API_URL}/api/v1/specialities/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!response.ok) {
    return false;
  }

  return true;
}
