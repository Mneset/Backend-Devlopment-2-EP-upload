import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAllPatientsIncludingInactive } from "@/services/patientService";
import { deactivatePatientAction, reactivatePatientAction } from "@/modules/patients/actions";
import type { Role } from "@/types/auth";

const FILTERS = [
  { label: "All", value: "all" },
  { label: "Active", value: "active" },
  { label: "Inactive", value: "inactive" },
];

export default async function AdminPatientsPage({
  searchParams,
}: {
  searchParams: Promise<{ status?: string }>;
}) {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  const token = await requireAuth();
  const patients = await getAllPatientsIncludingInactive(token);
  const { status } = await searchParams;

  if (patients === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load patients.</p>
      </div>
    );
  }

  const filtered =
    status === "active"
      ? patients.filter((p) => p.isActive)
      : status === "inactive"
        ? patients.filter((p) => !p.isActive)
        : patients;

  return (
    <div className="px-4 py-8 max-w-5xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-semibold text-blue-900">Patients</h1>
      </div>

      <div className="flex gap-2 mb-4">
        {FILTERS.map((f) => {
          const isActive = (status ?? "all") === f.value;
          const href = f.value === "all" ? "/admin/patients" : `/admin/patients?status=${f.value}`;
          return (
            <Link
              key={f.value}
              href={href}
              className={`px-3 py-1 rounded text-sm ${
                isActive ? "bg-blue-900 text-white" : "bg-white text-blue-900 border border-blue-900"
              }`}
            >
              {f.label}
            </Link>
          );
        })}
      </div>

      {filtered.length === 0 ? (
        <p className="text-gray-600">No patients to show.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">Name</th>
              <th className="p-3">Email</th>
              <th className="p-3">Type</th>
              <th className="p-3">Status</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((p) => (
              <tr key={p.id} className="border-t">
                <td className="p-3">{p.firstName} {p.lastName}</td>
                <td className="p-3">{p.email ?? <span className="text-gray-400">—</span>}</td>
                <td className="p-3">
                  {p.isRegistered ? "Registered" : <span className="text-gray-500">Guest</span>}
                </td>
                <td className="p-3">
                  {p.isActive ? (
                    <span className="text-green-700">Active</span>
                  ) : (
                    <span className="text-gray-500">Inactive</span>
                  )}
                </td>
                <td className="p-3 text-right space-x-2">
                  {p.isRegistered && (
                    <Link href={`/admin/patients/${p.id}`} className="text-blue-900 underline">
                      Edit
                    </Link>
                  )}
                  {p.isActive ? (
                    <form action={deactivatePatientAction} className="inline">
                      <input type="hidden" name="id" value={p.id} />
                      <button type="submit" className="text-red-700 underline">
                        Deactivate
                      </button>
                    </form>
                  ) : (
                    <form action={reactivatePatientAction} className="inline">
                      <input type="hidden" name="id" value={p.id} />
                      <button type="submit" className="text-green-700 underline">
                        Reactivate
                      </button>
                    </form>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
