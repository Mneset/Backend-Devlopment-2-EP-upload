"use client";

import { useActionState, useEffect, useState } from "react";
import { rescheduleAppointmentAction, type RescheduleState } from "@/modules/appointments/actions";

const initialState: RescheduleState = {};

type Props = {
  appointmentId: number;
  currentDate: string;
  currentDuration: number;
};

function toLocalDateTimeInput(iso: string): string {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

export default function RescheduleForm({ appointmentId, currentDate, currentDuration }: Props) {
  const [state, formAction, pending] = useActionState(
    rescheduleAppointmentAction,
    initialState,
  );

  const [appointmentDate, setAppointmentDate] = useState(toLocalDateTimeInput(currentDate));
  const [durationInMin, setDurationInMin] = useState(currentDuration);

  useEffect(() => {
    if (state.success) {
      alert("Appointment rescheduled");
      window.location.href = `/bookings/${appointmentId}`;
    }
  }, [state.success, appointmentId]);

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h2 className="text-xl font-semibold text-blue-900">Reschedule</h2>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <input type="hidden" name="id" value={appointmentId} />

      <label className="block">
        New date and time
        <input
          name="appointmentDate"
          type="datetime-local"
          required
          value={appointmentDate}
          onChange={(e) => setAppointmentDate(e.target.value)}
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
        {pending ? "Saving..." : "Save new time"}
      </button>
    </form>
  );
}
