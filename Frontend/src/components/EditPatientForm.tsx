"use client";

import { useActionState, useState } from "react";
import { updatePatientAction, type PatientFormState } from "@/modules/patients/actions";
import type { Gender, PatientResponseDto, Religion } from "@/types/patient";

const initialState: PatientFormState = {};

type Props = {
  initialData: PatientResponseDto;
};

export default function EditPatientForm({ initialData }: Props) {
  const [state, formAction, pending] = useActionState(
    updatePatientAction,
    initialState,
  );

  const [email, setEmail] = useState(initialData.email ?? "");
  const [firstName, setFirstName] = useState(initialData.firstName);
  const [lastName, setLastName] = useState(initialData.lastName);
  const [gender, setGender] = useState<Gender>(initialData.gender);
  const [dateOfBirth, setDateOfBirth] = useState(initialData.dateOfBirth.split("T")[0]);
  const [religion, setReligion] = useState<Religion>(initialData.religion ?? "PreferNotToSay");
  const [ssn, setSsn] = useState(initialData.socialSecurityNumber ?? "");
  const [tax, setTax] = useState(initialData.taxNumber ?? "");
  const [license, setLicense] = useState(initialData.driverLicenseNumber ?? "");
  const [insurance, setInsurance] = useState(initialData.medicalInsuranceNumber ?? "");

  const today = new Date().toISOString().split("T")[0];

  return (
    <form action={formAction} className="space-y-3 bg-white p-6 rounded shadow">
      <h1 className="text-2xl font-semibold text-blue-900">Edit patient</h1>

      {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

      <input type="hidden" name="id" value={initialData.id} />

      <label className="block">
        Email
        <input
          name="email"
          type="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

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
        Date of birth
        <input
          name="dateOfBirth"
          type="date"
          required
          max={today}
          value={dateOfBirth}
          onChange={(e) => setDateOfBirth(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Religion
        <select
          name="religion"
          value={religion}
          onChange={(e) => setReligion(e.target.value as Religion)}
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

      <label className="block">
        Social security number
        <input
          name="socialSecurityNumber"
          type="text"
          pattern="\d{11}"
          title="Must be exactly 11 digits"
          value={ssn}
          onChange={(e) => setSsn(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Tax number
        <input
          name="taxNumber"
          type="text"
          minLength={5}
          pattern="[A-Za-z0-9]+"
          title="Alphanumeric, min 5 characters"
          value={tax}
          onChange={(e) => setTax(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Driver license number
        <input
          name="driverLicenseNumber"
          type="text"
          minLength={5}
          pattern="[A-Za-z0-9]+"
          title="Alphanumeric, min 5 characters"
          value={license}
          onChange={(e) => setLicense(e.target.value)}
          className="w-full border rounded px-3 py-2"
        />
      </label>

      <label className="block">
        Medical insurance number
        <input
          name="medicalInsuranceNumber"
          type="text"
          minLength={5}
          pattern="[A-Za-z0-9]+"
          title="Alphanumeric, min 5 characters"
          value={insurance}
          onChange={(e) => setInsurance(e.target.value)}
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
