"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import {
  deactivatePatient,
  deleteOwnProfile,
  reactivatePatient,
  updateOwnProfile,
  updatePatient,
} from "@/services/patientService";
import type {
  Gender,
  Religion,
  UpdateOwnProfileRequestDto,
  UpdatePatientRequestDto,
} from "@/types/patient";

export type UpdateOwnProfileState = {
  error?: string;
  success?: boolean;
};

export async function updateOwnProfileAction(
  _prevState: UpdateOwnProfileState,
  formData: FormData,
): Promise<UpdateOwnProfileState> {
  const token = await requireAuth();

  const religionRaw = formData.get("religion") as string;
  const data: UpdateOwnProfileRequestDto = {
    email: formData.get("email") as string,
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    dateOfBirth: formData.get("dateOfBirth") as string,
    religion: religionRaw ? (religionRaw as Religion) : null,
  };

  const ok = await updateOwnProfile(data, token);
  if (!ok) {
    return { error: "Update failed. The email may already be taken." };
  }

  revalidatePath("/profile");
  return { success: true };
}

export async function deleteOwnProfileAction() {
  const token = await requireAuth();
  await deleteOwnProfile(token);

  const cookieStore = await cookies();
  cookieStore.delete("token");
  cookieStore.delete("role");
  redirect("/");
}

export type PatientFormState = {
  error?: string;
};

export async function updatePatientAction(
  _prevState: PatientFormState,
  formData: FormData,
): Promise<PatientFormState> {
  const token = await requireAuth();

  const religionRaw = formData.get("religion") as string;
  const ssn = formData.get("socialSecurityNumber") as string;
  const tax = formData.get("taxNumber") as string;
  const license = formData.get("driverLicenseNumber") as string;
  const insurance = formData.get("medicalInsuranceNumber") as string;

  const data: UpdatePatientRequestDto = {
    id: Number(formData.get("id")),
    email: formData.get("email") as string,
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    dateOfBirth: formData.get("dateOfBirth") as string,
    religion: religionRaw ? (religionRaw as Religion) : null,
    socialSecurityNumber: ssn || null,
    taxNumber: tax || null,
    driverLicenseNumber: license || null,
    medicalInsuranceNumber: insurance || null,
  };

  const ok = await updatePatient(data, token);
  if (!ok) {
    return { error: "Update failed. The email may already be taken." };
  }

  revalidatePath("/admin/patients");
  revalidatePath(`/admin/patients/${data.id}`);
  redirect("/admin/patients");
}

export async function deactivatePatientAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await deactivatePatient(id, token);
  revalidatePath("/admin/patients");
}

export async function reactivatePatientAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));
  
  await reactivatePatient(id, token);
  revalidatePath("/admin/patients");
}
