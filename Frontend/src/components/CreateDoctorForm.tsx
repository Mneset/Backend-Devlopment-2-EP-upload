"use client";

import { useActionState, useState } from "react";
import { createDoctorAction, type DoctorFormState } from "@/modules/doctors/actions";
import type { ClinicResponseDto } from "@/types/clinic";
import type { SpecialityResponseDto } from "@/types/speciality";
import type { Gender } from "@/types/patient";

const initialState: DoctorFormState = {};

type Props = {
  clinics: ClinicResponseDto[];
  specialities: SpecialityResponseDto[];
};

export default function CreateDoctorForm({ clinics, specialities }: Props) {
  const [state, formAction, pending] = useActionState(
    createDoctorAction,
    initialState,
  );

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [gender, setGender] = useState<Gender>("Male");
  const [specialityId, setSpecialityId] = useState<number | "">("");
  const [clinicId, setClinicId] = useState<number | "">("");

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h1 className="text-2xl font-semibold text-blue-900">New doctor</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <label className="block">
        First name
        <input
          name="firstName"
          type="text"
          required
          minLength={1}
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Last name
        <input
          name="lastName"
          type="text"
          required
          minLength={1}
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Gender
        <select
          name="gender"
          required
          value={gender}
          onChange={(e) => setGender(e.target.value as Gender)}
          className="w-full border rounded px-3 py-2"
        >
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
      </label>

      <label className="block">
        Speciality
        <select
          name="specialityId"
          required
          value={specialityId}
          onChange={(e) => setSpecialityId(Number(e.target.value))}
          className="w-full border rounded px-3 py-2"
        >
          <option value="">Select a speciality</option>
          {specialities.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name}
            </option>
          ))}
        </select>
      </label>

      <label className="block">
        Clinic
        <select
          name="clinicId"
          required
          value={clinicId}
          onChange={(e) => setClinicId(Number(e.target.value))}
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
