"use client";

import { useActionState } from "react";
import { loginAction, type LoginState } from "@/modules/auth/actions";

const initialState: LoginState = {};

export default function LoginPage() {
  const [state, formAction, pending] = useActionState(loginAction, initialState);

  return (
    <div className="flex justify-center px-4 py-8">
      <form
        action={formAction}
        className="w-full max-w-sm space-y-4 bg-white p-6 rounded shadow"
      >
        <h1 className="text-2xl font-semibold text-blue-900">Log in</h1>

        {state.error && <p className="text-red-600 text-sm">{state.error}</p>}

        <div>
          <label htmlFor="email" className="block text-sm font-medium mb-1">
            Email
          </label>
          <input
            id="email"
            name="email"
            type="email"
            required
            className="w-full border rounded px-3 py-2"
          />
        </div>

        <div>
          <label htmlFor="password" className="block text-sm font-medium mb-1">
            Password
          </label>
          <input
            id="password"
            name="password"
            type="password"
            required
            className="w-full border rounded px-3 py-2"
          />
        </div>

        <button
          type="submit"
          disabled={pending}
          className="w-full bg-blue-900 text-white py-2 rounded hover:bg-blue-950 disabled:opacity-50"
        >
          {pending ? "Logging in..." : "Log in"}
        </button>
      </form>
    </div>
  );
}
