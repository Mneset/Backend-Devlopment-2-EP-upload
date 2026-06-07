"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import {
  createAppointmentCategory,
  deleteAppointmentCategory,
  updateAppointmentCategory,
} from "@/services/appointmentCategoryService";
import type { CreateAppointmentCategoryRequestDto, UpdateAppointmentCategoryRequestDto } from "@/types/appointmentCategory";

export type AppointmentCategoryFormState = {
  error?: string;
};

export async function createAppointmentCategoryAction(
  _prevState: AppointmentCategoryFormState,
  formData: FormData,
): Promise<AppointmentCategoryFormState> {
  const token = await requireAuth();

  const data: CreateAppointmentCategoryRequestDto = {
    name: formData.get("name") as string,
  };

  const ok = await createAppointmentCategory(data, token);
  if (!ok) {
    return { error: "Could not create appointment category." };
  }

  revalidatePath("/admin/appointment-categories");
  redirect("/admin/appointment-categories");
}

export async function updateAppointmentCategoryAction(
  _prevState: AppointmentCategoryFormState,
  formData: FormData,
): Promise<AppointmentCategoryFormState> {
  const token = await requireAuth();

  const data: UpdateAppointmentCategoryRequestDto = {
    id: Number(formData.get("id")),
    name: formData.get("name") as string,
  };

  const ok = await updateAppointmentCategory(data, token);
  if (!ok) {
    return { error: "Could not update appointment category." };
  }

  revalidatePath("/admin/appointment-categories");
  redirect("/admin/appointment-categories");
}

export async function deleteAppointmentCategoryAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await deleteAppointmentCategory(id, token);
  revalidatePath("/admin/appointment-categories");
}
