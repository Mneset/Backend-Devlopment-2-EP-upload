import { API_URL } from "@/lib/constants";
import type { RegisterPatientRequestDto } from "@/types/patient";

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
