import { API_URL } from "@/lib/constants";
import type { AppointmentCategoryResponseDto } from "@/types/appointmentCategory";

export async function getAllAppointmentCategories(): Promise<
  AppointmentCategoryResponseDto[] | null
> {
  const response = await fetch(`${API_URL}/api/v1/appointmentcategories`);

  if (!response.ok) {
    return null;
  }

  return response.json();
}
