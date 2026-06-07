"use client";

import { useActionState, useState } from "react";
import { bookAppointmentAction, type BookAppointmentState } from "@/modules/appointments/actions";
import type { ClinicResponseDto } from "@/types/clinic";
import type { DoctorResponseDto } from "@/types/doctor";
import type { AppointmentCategoryResponseDto } from "@/types/appointmentCategory";

const initialState: BookAppointmentState = {};

type Props = {
  clinics: ClinicResponseDto[];
  doctors: DoctorResponseDto[];
  categories: AppointmentCategoryResponseDto[];
};

export default function BookAppointmentForm({ clinics, doctors, categories }: Props) {
  const [state, formAction, pending] = useActionState(
    bookAppointmentAction,
    initialState,
  );

  const [clinicId, setClinicId] = useState<number | "">("");
  const [doctorId, setDoctorId] = useState<number | "">("");
  const [categoryId, setCategoryId] = useState<number | "">("");
  const [date, setDate] = useState("");
  const [time, setTime] = useState("");
  const [durationInMin, setDurationInMin] = useState(30);

  const today = new Date().toISOString().split("T")[0];
  const appointmentDate = date && time ? `${date}T${time}` : "";

  const doctorsAtClinic = doctors.filter((d) => d.clinicId === clinicId);

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h1 className="text-2xl font-semibold text-blue-900">Book an appointment</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <label className="block">
        Clinic
        <select
          name="clinicId"
          required
          value={clinicId}
          onChange={(e) => {
            setClinicId(Number(e.target.value));
            setDoctorId("");
          }}
          className="w-full border rounded px-3 py-2"
        >
          <option value="">Select a clinic</option>
          {clinics.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      </label>

      {clinicId !== "" && (
        <label className="block">
          Doctor
          <select
            name="doctorId"
            required
            value={doctorId}
            onChange={(e) => setDoctorId(Number(e.target.value))}
            className="w-full border rounded px-3 py-2"
          >
            <option value="">Select a doctor</option>
            {doctorsAtClinic.map((d) => (
              <option key={d.id} value={d.id}>
                {d.firstName} {d.lastName} ({d.specialityName})
              </option>
            ))}
          </select>
          {doctorsAtClinic.length === 0 && (
            <p className="text-sm text-gray-600 mt-1">No doctors available at this clinic.</p>
          )}
        </label>
      )}

      {doctorId !== "" && (
        <>
          <label className="block">
            Appointment type
            <select
              name="appointmentCategoryId"
              required
              value={categoryId}
              onChange={(e) => setCategoryId(Number(e.target.value))}
              className="w-full border rounded px-3 py-2"
            >
              <option value="">Select a type</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>

          <input type="hidden" name="appointmentDate" value={appointmentDate} />

          <label className="block">
            Date
            <input
              type="date"
              required
              min={today}
              value={date}
              onChange={(e) => setDate(e.target.value)}
              className="w-full border rounded px-3 py-2"
            />
          </label>

          <label className="block">
            Time (between 8 AM and 6 PM)
            <input
              type="time"
              required
              min="08:00"
              max="18:00"
              value={time}
              onChange={(e) => setTime(e.target.value)}
              className="w-full border rounded px-3 py-2"
            />
          </label>

          <label className="block">
            Duration (minutes)
            <input
              name="durationInMin"
              type="number"
              required
              min={1}
              max={480}
              value={durationInMin}
              onChange={(e) => setDurationInMin(Number(e.target.value))}
              className="w-full border rounded px-3 py-2"
            />
          </label>

          <button
            type="submit"
            disabled={pending}
            className="w-full bg-blue-900 text-white py-2 rounded hover:bg-blue-950 disabled:opacity-50"
          >
            {pending ? "Booking..." : "Book appointment"}
          </button>
        </>
      )}
    </form>
  );
}
