"use client";

import { useActionState, useEffect } from "react";
import { updateOwnProfileAction, type UpdateOwnProfileState } from "@/modules/patients/actions";
import type { MyProfileResponseDto } from "@/types/patient";

const initialState: UpdateOwnProfileState = {};

type Props = {
  initialData: MyProfileResponseDto;
};

export default function ProfileForm({ initialData }: Props) {
  const [state, formAction, pending] = useActionState(
    updateOwnProfileAction,
    initialState,
  );

  useEffect(() => {
    if (state.success) {
      alert("Profile updated");
      window.location.href = "/profile";
    }
  }, [state.success]);

  const today = new Date().toISOString().split("T")[0];

  return (
    <form
      action={formAction}
      className="space-y-3 bg-white p-6 rounded shadow"
    >
      <h1 className="text-2xl font-semibold text-blue-900">My profile</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <label className="block">
        Email
        <input
          name="email"
          type="email"
          required
          defaultValue={initialData.email}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        First name
        <input
          name="firstName"
          type="text"
          required
          defaultValue={initialData.firstName}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Last name
        <input
          name="lastName"
          type="text"
          required
          defaultValue={initialData.lastName}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Gender
        <select
          name="gender"
          required
          defaultValue={initialData.gender}
          className="w-full border rounded px-3 py-2"
        >
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
      </label>

      <label className="block">
        Date of birth
        <input
          name="dateOfBirth"
          type="date"
          required
          max={today}
          defaultValue={initialData.dateOfBirth.split("T")[0]}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Religion
        <select
          name="religion"
          defaultValue={initialData.religion ?? "PreferNotToSay"}
          className="w-full border rounded px-3 py-2"
        >
          <option value="None">None</option>
          <option value="Christianity">Christianity</option>
          <option value="Islam">Islam</option>
          <option value="Judaism">Judaism</option>
          <option value="Hinduism">Hinduism</option>
          <option value="Buddhism">Buddhism</option>
          <option value="Other">Other</option>
          <option value="PreferNotToSay">Prefer not to say</option>
        </select>
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
