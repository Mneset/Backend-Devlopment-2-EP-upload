import { API_URL } from "@/lib/constants";
import type { DoctorResponseDto } from "@/types/doctor";

export async function getAllDoctors(): Promise<DoctorResponseDto[] | null> {
  const response = await fetch(`${API_URL}/api/v1/doctors`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}
