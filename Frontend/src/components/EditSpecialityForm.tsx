"use client";

import { useActionState, useState } from "react";
import { updateSpecialityAction, type SpecialityFormState } from "@/modules/specialities/actions";
import type { SpecialityResponseDto } from "@/types/speciality";

const initialState: SpecialityFormState = {};

type Props = {
  initialData: SpecialityResponseDto;
};

export default function EditSpecialityForm({ initialData }: Props) {
  const [state, formAction, pending] = useActionState(
    updateSpecialityAction,
    initialState,
  );

  const [name, setName] = useState(initialData.name);

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h1 className="text-2xl font-semibold text-blue-900">Edit speciality</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <input type="hidden" name="id" value={initialData.id} />

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
        {pending ? "Saving..." : "Save changes"}
      </button>
    </form>
  );
}
