"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import {
  createDoctor,
  deactivateDoctor,
  reactivateDoctor,
  updateDoctor,
} from "@/services/doctorService";
import type { CreateDoctorRequestDto, UpdateDoctorRequestDto } from "@/types/doctor";
import type { Gender } from "@/types/patient";

export type DoctorFormState = {
  error?: string;
};

export async function createDoctorAction(
  _prevState: DoctorFormState,
  formData: FormData,
): Promise<DoctorFormState> {
  const token = await requireAuth();

  const data: CreateDoctorRequestDto = {
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    specialityId: Number(formData.get("specialityId")),
    clinicId: Number(formData.get("clinicId")),
  };

  const ok = await createDoctor(data, token);
  if (!ok) {
    return { error: "Could not create doctor." };
  }

  revalidatePath("/admin/doctors");
  redirect("/admin/doctors");
}

export async function updateDoctorAction(
  _prevState: DoctorFormState,
  formData: FormData,
): Promise<DoctorFormState> {
  const token = await requireAuth();

  const data: UpdateDoctorRequestDto = {
    id: Number(formData.get("id")),
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    specialityId: Number(formData.get("specialityId")),
    clinicId: Number(formData.get("clinicId")),
  };

  const ok = await updateDoctor(data, token);
  if (!ok) {
    return { error: "Could not update doctor." };
  }

  revalidatePath("/admin/doctors");
  redirect("/admin/doctors");
}

export async function deactivateDoctorAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await deactivateDoctor(id, token);
  revalidatePath("/admin/doctors");
}

export async function reactivateDoctorAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await reactivateDoctor(id, token);
  revalidatePath("/admin/doctors");
}
