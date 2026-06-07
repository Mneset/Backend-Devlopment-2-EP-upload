import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAllClinicsIncludingInactive } from "@/services/clinicService";
import { deactivateClinicAction, reactivateClinicAction } from "@/modules/clinics/actions";
import type { Role } from "@/types/auth";

const FILTERS = [
  { label: "All", value: "all" },
  { label: "Active", value: "active" },
  { label: "Inactive", value: "inactive" },
];

export default async function AdminClinicsPage({
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
  const clinics = await getAllClinicsIncludingInactive(token);
  const { status } = await searchParams;

  if (clinics === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load clinics.</p>
      </div>
    );
  }

  const filtered =
    status === "active"
      ? clinics.filter((c) => c.isActive)
      : status === "inactive"
        ? clinics.filter((c) => !c.isActive)
        : clinics;

  return (
    <div className="px-4 py-8 max-w-4xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-semibold text-blue-900">Clinics</h1>
        <Link
          href="/admin/clinics/new"
          className="bg-blue-900 text-white px-4 py-2 rounded hover:bg-blue-950"
        >
          New clinic
        </Link>
      </div>

      <div className="flex gap-2 mb-4">
        {FILTERS.map((f) => {
          const isActive = (status ?? "all") === f.value;
          const href = f.value === "all" ? "/admin/clinics" : `/admin/clinics?status=${f.value}`;
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
        <p className="text-gray-600">No clinics to show.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">Name</th>
              <th className="p-3">Status</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((c) => (
              <tr key={c.id} className="border-t">
                <td className="p-3">{c.name}</td>
                <td className="p-3">
                  {c.isActive ? (
                    <span className="text-green-700">Active</span>
                  ) : (
                    <span className="text-gray-500">Inactive</span>
                  )}
                </td>
                <td className="p-3 text-right space-x-2">
                  <Link
                    href={`/admin/clinics/${c.id}`}
                    className="text-blue-900 underline"
                  >
                    Edit
                  </Link>
                  {c.isActive ? (
                    <form
                      action={deactivateClinicAction}
                      className="inline"
                    >
                      <input type="hidden" name="id" value={c.id} />
                      <button
                        type="submit"
                        className="text-red-700 underline"
                      >
                        Deactivate
                      </button>
                    </form>
                  ) : (
                    <form
                      action={reactivateClinicAction}
                      className="inline"
                    >
                      <input type="hidden" name="id" value={c.id} />
                      <button
                        type="submit"
                        className="text-green-700 underline"
                      >
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
