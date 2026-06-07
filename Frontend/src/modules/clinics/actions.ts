"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import {
  createClinic,
  deactivateClinic,
  reactivateClinic,
  updateClinic,
} from "@/services/clinicService";
import type { CreateClinicRequestDto, UpdateClinicRequestDto } from "@/types/clinic";

export type ClinicFormState = {
  error?: string;
};

export async function createClinicAction(
  _prevState: ClinicFormState,
  formData: FormData,
): Promise<ClinicFormState> {
  const token = await requireAuth();

  const data: CreateClinicRequestDto = {
    name: formData.get("name") as string,
  };

  const ok = await createClinic(data, token);
  if (!ok) {
    return { error: "Could not create clinic." };
  }

  revalidatePath("/admin/clinics");
  redirect("/admin/clinics");
}

export async function updateClinicAction(
  _prevState: ClinicFormState,
  formData: FormData,
): Promise<ClinicFormState> {
  const token = await requireAuth();

  const data: UpdateClinicRequestDto = {
    id: Number(formData.get("id")),
    name: formData.get("name") as string,
  };

  const ok = await updateClinic(data, token);
  if (!ok) {
    return { error: "Could not update clinic." };
  }

  revalidatePath("/admin/clinics");
  redirect("/admin/clinics");
}

export async function deactivateClinicAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await deactivateClinic(id, token);
  revalidatePath("/admin/clinics");
}

export async function reactivateClinicAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await reactivateClinic(id, token);
  revalidatePath("/admin/clinics");
}
