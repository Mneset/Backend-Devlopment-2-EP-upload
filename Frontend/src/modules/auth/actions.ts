"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { login } from "@/services/authService";
import { register } from "@/services/patientService";
import type { Gender, Religion, RegisterPatientRequestDto } from "@/types/patient";

export type LoginState = {
  error?: string;
};

export async function loginAction(
  _prevState: LoginState,
  formData: FormData,
): Promise<LoginState> {
  const email = formData.get("email") as string;
  const password = formData.get("password") as string;

  const result = await login({ email, password });
  if (result === null) {
    return { error: "Invalid email or password." };
  }

  const cookieStore = await cookies();
  const cookieOptions = {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/",
    expires: new Date(result.expiresAt),
  };
  cookieStore.set("token", result.token, cookieOptions);
  cookieStore.set("role", result.role, cookieOptions);

  if (result.role === "Admin") redirect("/admin");
  if (result.role === "Patient") redirect("/");
  redirect("/");
}

export async function logoutAction() {
  const cookieStore = await cookies();
  cookieStore.delete("token");
  cookieStore.delete("role");
  redirect("/login");
}

export type RegisterState = {
  error?: string;
};

export async function registerAction(
  _prevState: RegisterState,
  formData: FormData,
): Promise<RegisterState> {
  const data: RegisterPatientRequestDto = {
    email: formData.get("email") as string,
    password: formData.get("password") as string,
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    dateOfBirth: formData.get("dateOfBirth") as string,
    religion: formData.get("religion") as Religion,
    socialSecurityNumber: formData.get("socialSecurityNumber") as string,
    taxNumber: formData.get("taxNumber") as string,
    driverLicenseNumber: formData.get("driverLicenseNumber") as string,
    medicalInsuranceNumber: formData.get("medicalInsuranceNumber") as string,
  };

  const created = await register(data);
  if (!created) {
    return { error: "Registration failed. The email may already be in use." };
  }

  const tokenResponse = await login({
    email: data.email,
    password: data.password,
  });
  if (tokenResponse === null) {
    return {
      error: "Account created, but login failed. Please go to the login page.",
    };
  }

  const cookieStore = await cookies();
  const cookieOptions = {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax" as const,
    path: "/",
    expires: new Date(tokenResponse.expiresAt),
  };
  cookieStore.set("token", tokenResponse.token, cookieOptions);
  cookieStore.set("role", tokenResponse.role, cookieOptions);

  redirect("/");
}
