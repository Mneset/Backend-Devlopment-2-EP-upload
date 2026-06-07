"use client";

import { useActionState, useState } from "react";
import { createAppointmentCategoryAction, type AppointmentCategoryFormState } from "@/modules/appointmentCategories/actions";

const initialState: AppointmentCategoryFormState = {};

export default function CreateAppointmentCategoryForm() {
  const [state, formAction, pending] = useActionState(
    createAppointmentCategoryAction,
    initialState,
  );

  const [name, setName] = useState("");

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h1 className="text-2xl font-semibold text-blue-900">New appointment category</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <label className="block">
        Name
        <input
          name="name"
          type="text"
          required
          minLength={1}
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <button
        type="submit"
        disabled={pending}
        className="w-full bg-blue-900 text-white py-2 rounded hover:bg-blue-950 disabled:opacity-50"
      >
        {pending ? "Creating..." : "Create"}
      </button>
    </form>
  );
}
