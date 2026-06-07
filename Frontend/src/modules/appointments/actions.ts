"use server";

import { redirect } from "next/navigation";
import { revalidatePath } from "next/cache";
import { requireAuth } from "@/lib/auth";
import { bookAppointment, bookGuestAppointment, cancelAppointment, rescheduleAppointment } from "@/services/appointmentService";
import type { BookAppointmentRequestDto, BookGuestAppointmentRequestDto, RescheduleAppointmentRequestDto } from "@/types/appointment";
import type { Gender } from "@/types/patient";

export type BookGuestAppointmentState = {
  error?: string;
  success?: boolean;
};

export async function bookGuestAppointmentAction(
  _prevState: BookGuestAppointmentState,
  formData: FormData,
): Promise<BookGuestAppointmentState> {
  const data: BookGuestAppointmentRequestDto = {
    firstName: formData.get("firstName") as string,
    lastName: formData.get("lastName") as string,
    gender: formData.get("gender") as Gender,
    dateOfBirth: formData.get("dateOfBirth") as string,
    doctorId: Number(formData.get("doctorId")),
    clinicId: Number(formData.get("clinicId")),
    appointmentCategoryId: Number(formData.get("appointmentCategoryId")),
    appointmentDate: formData.get("appointmentDate") as string,
    durationInMin: Number(formData.get("durationInMin")),
  };

  const result = await bookGuestAppointment(data);
  if (!result.success) {
    if (result.status === 409) {
      return { error: "This time slot conflicts with another appointment. Please pick a different time." };
    }
    return { error: "Booking failed. Please re-check your details and try again." };
  }

  return { success: true };
}

export type BookAppointmentState = {
  error?: string;
};

export async function bookAppointmentAction(
  _prevState: BookAppointmentState,
  formData: FormData,
): Promise<BookAppointmentState> {
  const token = await requireAuth();

  const data: BookAppointmentRequestDto = {
    doctorId: Number(formData.get("doctorId")),
    clinicId: Number(formData.get("clinicId")),
    appointmentCategoryId: Number(formData.get("appointmentCategoryId")),
    appointmentDate: formData.get("appointmentDate") as string,
    durationInMin: Number(formData.get("durationInMin")),
  };

  const result = await bookAppointment(data, token);
  if (!result.success) {
    if (result.status === 409) {
      return { error: "This time slot conflicts with another appointment. Please pick a different time." };
    }
    if (result.status === 403) {
      return { error: "You are not authorized. Please log in again." };
    }
    return { error: "Booking failed. Please re-check your selections and try again." };
  }

  revalidatePath("/bookings");
  redirect(`/bookings/${result.data.id}`);
}

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
