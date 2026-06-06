import { API_URL } from "@/lib/constants";
import type { LoginRequestDto, TokenResponseDto } from "@/types/auth";

export async function login(
  data: LoginRequestDto,
): Promise<TokenResponseDto | null> {
  const response = await fetch(`${API_URL}/api/v1/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    return null;
  }

  return response.json();
}
