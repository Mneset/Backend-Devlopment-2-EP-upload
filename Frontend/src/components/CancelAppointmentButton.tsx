"use client";

import { cancelAppointmentAction } from "@/modules/appointments/actions";

type Props = {
  appointmentId: number;
};

export default function CancelAppointmentButton({ appointmentId }: Props) {
  return (
    <form
      action={cancelAppointmentAction}
      onSubmit={(e) => {
        if (!confirm("Cancel this appointment?")) {
          e.preventDefault();
        }
      }}
    >
      <input type="hidden" name="id" value={appointmentId} />
      <button
        type="submit"
        className="w-full bg-red-700 text-white py-2 rounded hover:bg-red-800"
      >
        Cancel appointment
      </button>
    </form>
  );
}
