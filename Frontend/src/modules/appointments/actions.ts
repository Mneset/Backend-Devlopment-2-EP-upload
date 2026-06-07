"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import { cancelAppointment, rescheduleAppointment } from "@/services/appointmentService";
import type { RescheduleAppointmentRequestDto } from "@/types/appointment";

export async function cancelAppointmentAction(formData: FormData) {
  const token = await requireAuth();
  const id = Number(formData.get("id"));

  await cancelAppointment(id, token);
  revalidatePath(`/bookings/${id}`);
  revalidatePath("/bookings");
  redirect(`/bookings/${id}`);
}

export type RescheduleState = {
  error?: string;
  success?: boolean;
};

export async function rescheduleAppointmentAction(
  _prevState: RescheduleState,
  formData: FormData,
): Promise<RescheduleState> {
  const token = await requireAuth();

  const data: RescheduleAppointmentRequestDto = {
    id: Number(formData.get("id")),
    appointmentDate: formData.get("appointmentDate") as string,
    durationInMin: Number(formData.get("durationInMin")),
  };

  const ok = await rescheduleAppointment(data, token);
  if (!ok) {
    return { error: "Reschedule failed. The new time may overlap with another appointment." };
  }

  revalidatePath(`/bookings/${data.id}`);
  revalidatePath("/bookings");
  return { success: true };
}
