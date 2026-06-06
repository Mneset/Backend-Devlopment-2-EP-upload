export type Role = "Admin" | "Patient" | "Guest";

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface TokenResponseDto {
  token: string;
  role: Role;
  expiresAt: string;
}
