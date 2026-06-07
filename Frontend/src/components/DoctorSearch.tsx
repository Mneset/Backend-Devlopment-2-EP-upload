"use client";

import { useState } from "react";
import type { DoctorResponseDto } from "@/types/doctor";

type Props = {
  doctors: DoctorResponseDto[];
};

export default function DoctorSearch({ doctors }: Props) {
  const [query, setQuery] = useState("");

  const trimmed = query.trim().toLowerCase();
  const filtered = trimmed
    ? doctors.filter(
        (d) =>
          d.firstName.toLowerCase().includes(trimmed) ||
          d.lastName.toLowerCase().includes(trimmed),
      )
    : doctors;

  return (
    <div className="space-y-4">
      <input
        type="text"
        placeholder="Search by first or last name"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        className="w-full border rounded px-3 py-2"
      />

      {filtered.length === 0 ? (
        <p className="text-gray-600 text-center py-6">
          No doctors found matching &ldquo;{query}&rdquo;.
        </p>
      ) : (
        <ul className="space-y-2">
          {filtered.map((d) => (
            <li key={d.id} className="bg-white p-4 rounded shadow">
              <p className="font-semibold text-blue-900">
                {d.firstName} {d.lastName}
              </p>
              <p className="text-sm text-gray-700">{d.specialityName}</p>
              <p className="text-sm text-gray-700">{d.clinicName}</p>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
