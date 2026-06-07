"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import {
  createSpeciality,
  deleteSpeciality,
  updateSpeciality,
} from "@/services/specialityService";
import type { CreateSpecialityRequestDto, UpdateSpecialityRequestDto } from "@/types/speciality";

export type SpecialityFormState = {
  error?: string;
};

export async function createSpecialityAction(
  _prevState: SpecialityFormState,
  formData: FormData,
): Promise<SpecialityFormState> {
  const token = await requireAuth();

  const data: CreateSpecialityRequestDto = {
    name: formData.get("name") as string,
  };

  const ok = await createSpeciality(data, token);
  if (!ok) {
    return { error: "Could not create speciality." };
  }

  revalidatePath("/admin/specialities");
  redirect("/admin/specialities");
}

export async function updateSpecialityAction(
  _prevState: SpecialityFormState,
  formData: FormData,
): Promise<SpecialityFormState> {
  const token = await requireAuth();

  const data: UpdateSpecialityRequestDto = {
    id: Number(formData.get("id")),
    name: formData.get("name") as string,
  };

  const ok = await updateSpeciality(data, token);
  if (!ok) {
    return { error: "Could not update speciality." };
  }

  revalidatePath("/admin/specialities");
  redirect("/admin/specialities");
}

export async function deleteSpecialityAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await deleteSpeciality(id, token);
  revalidatePath("/admin/specialities");
}
