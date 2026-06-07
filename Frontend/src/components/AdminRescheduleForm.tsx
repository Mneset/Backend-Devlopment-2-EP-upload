"use client";

import { useActionState, useEffect, useState } from "react";
import { adminRescheduleAppointmentAction, type RescheduleState } from "@/modules/appointments/actions";

const initialState: RescheduleState = {};

type Props = {
  appointmentId: number;
  currentDate: string;
  currentDuration: number;
};

function splitDateTime(iso: string): { date: string; time: string } {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, "0");
  return {
    date: `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`,
    time: `${pad(d.getHours())}:${pad(d.getMinutes())}`,
  };
}

export default function AdminRescheduleForm({ appointmentId, currentDate, currentDuration }: Props) {
  const [state, formAction, pending] = useActionState(
    adminRescheduleAppointmentAction,
    initialState,
  );

  const initial = splitDateTime(currentDate);
  const [date, setDate] = useState(initial.date);
  const [time, setTime] = useState(initial.time);
  const [durationInMin, setDurationInMin] = useState(currentDuration);

  const today = new Date().toISOString().split("T")[0];
  const appointmentDate = date && time ? `${date}T${time}` : "";

  useEffect(() => {
    if (state.success) {
      alert("Appointment rescheduled");
      window.location.href = `/admin/appointments/${appointmentId}`;
    }
  }, [state.success, appointmentId]);

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h2 className="text-xl font-semibold text-blue-900">Reschedule</h2>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <input type="hidden" name="id" value={appointmentId} />
      <input type="hidden" name="appointmentDate" value={appointmentDate} />

      <label className="block">
        New date
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
        New time (between 8 AM and 6 PM)
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
        {pending ? "Saving..." : "Save new time"}
      </button>
    </form>
  );
}
