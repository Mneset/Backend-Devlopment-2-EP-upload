"use client";

import { useActionState } from "react";
import { registerAction, type RegisterState } from "@/modules/auth/actions";

const initialState: RegisterState = {};

export default function RegisterPage() {
  const [state, formAction, pending] = useActionState(
    registerAction,
    initialState,
  );

  const today = new Date().toISOString().split("T")[0];

  return (
    <div className="flex justify-center px-4 py-8">
      <form
        action={formAction}
        className="w-full max-w-md space-y-3 bg-white p-6 rounded shadow"
      >
        <h1 className="text-2xl font-semibold text-blue-900">Register</h1>

        {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

        <label className="block">
          Email
          <input name="email" type="email" required className="w-full border rounded px-3 py-2" />
        </label>

        <label className="block">
          Password
          <input
            name="password"
            type="password"
            required
            minLength={8}
            pattern="(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}"
            title="At least 8 characters with one uppercase, one lowercase, and one digit"
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <label className="block">
          First name
          <input name="firstName" type="text" required className="w-full border rounded px-3 py-2" />
        </label>

        <label className="block">
          Last name
          <input name="lastName" type="text" required className="w-full border rounded px-3 py-2" />
        </label>

        <label className="block">
          Gender
          <select name="gender" required className="w-full border rounded px-3 py-2">
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
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <label className="block">
          Religion
          <select name="religion" required defaultValue="PreferNotToSay" className="w-full border rounded px-3 py-2">
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
            required
            pattern="\d{11}"
            maxLength={11}
            title="Exactly 11 digits"
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <label className="block">
          Tax number
          <input
            name="taxNumber"
            type="text"
            required
            minLength={5}
            pattern="[A-Za-z0-9]+"
            title="At least 5 alphanumeric characters"
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <label className="block">
          Driver license number
          <input
            name="driverLicenseNumber"
            type="text"
            required
            minLength={5}
            pattern="[A-Za-z0-9]+"
            title="At least 5 alphanumeric characters"
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <label className="block">
          Medical insurance number
          <input
            name="medicalInsuranceNumber"
            type="text"
            required
            minLength={5}
            pattern="[A-Za-z0-9]+"
            title="At least 5 alphanumeric characters"
            className="w-full border rounded px-3 py-2"
          />
        </label>

        <button
          type="submit"
          disabled={pending}
          className="w-full bg-blue-900 text-white py-2 rounded hover:bg-blue-950 disabled:opacity-50"
        >
          {pending ? "Registering..." : "Register"}
        </button>
      </form>
    </div>
  );
}
