"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import { deleteOwnProfile, updateOwnProfile } from "@/services/patientService";
import type { Gender, Religion, UpdateOwnProfileRequestDto } from "@/types/patient";

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
