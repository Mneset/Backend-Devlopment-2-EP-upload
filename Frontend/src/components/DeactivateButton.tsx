"use client";

import { deleteOwnProfileAction } from "@/modules/patients/actions";

export default function DeactivateButton() {
  return (
    <form
      action={deleteOwnProfileAction}
      onSubmit={(e) => {
        if (
          !confirm(
            "Are you sure you want to deactivate your account? This will cancel all your scheduled appointments and log you out.",
          )
        ) {
          e.preventDefault();
        }
      }}
    >
      <button
        type="submit"
        className="w-full bg-red-700 text-white py-2 rounded hover:bg-red-800"
      >
        Deactivate my account
      </button>
    </form>
  );
}
