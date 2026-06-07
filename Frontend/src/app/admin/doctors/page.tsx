import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAllDoctorsIncludingInactive } from "@/services/doctorService";
import { deactivateDoctorAction, reactivateDoctorAction } from "@/modules/doctors/actions";
import type { Role } from "@/types/auth";

const FILTERS = [
  { label: "All", value: "all" },
  { label: "Active", value: "active" },
  { label: "Inactive", value: "inactive" },
];

export default async function AdminDoctorsPage({
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
  const doctors = await getAllDoctorsIncludingInactive(token);
  const { status } = await searchParams;

  if (doctors === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load doctors.</p>
      </div>
    );
  }

  const filtered =
    status === "active"
      ? doctors.filter((d) => d.isActive)
      : status === "inactive"
        ? doctors.filter((d) => !d.isActive)
        : doctors;

  return (
    <div className="px-4 py-8 max-w-4xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-semibold text-blue-900">Doctors</h1>
        <Link
          href="/admin/doctors/new"
          className="bg-blue-900 text-white px-4 py-2 rounded hover:bg-blue-950"
        >
          New doctor
        </Link>
      </div>

      <div className="flex gap-2 mb-4">
        {FILTERS.map((f) => {
          const isActive = (status ?? "all") === f.value;
          const href = f.value === "all" ? "/admin/doctors" : `/admin/doctors?status=${f.value}`;
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
        <p className="text-gray-600">No doctors to show.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">Name</th>
              <th className="p-3">Speciality</th>
              <th className="p-3">Clinic</th>
              <th className="p-3">Status</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((d) => (
              <tr key={d.id} className="border-t">
                <td className="p-3">{d.firstName} {d.lastName}</td>
                <td className="p-3">{d.specialityName}</td>
                <td className="p-3">{d.clinicName}</td>
                <td className="p-3">
                  {d.isActive ? (
                    <span className="text-green-700">Active</span>
                  ) : (
                    <span className="text-gray-500">Inactive</span>
                  )}
                </td>
                <td className="p-3 text-right space-x-2">
                  <Link href={`/admin/doctors/${d.id}`} className="text-blue-900 underline">
                    Edit
                  </Link>
                  {d.isActive ? (
                    <form action={deactivateDoctorAction} className="inline">
                      <input type="hidden" name="id" value={d.id} />
                      <button type="submit" className="text-red-700 underline">
                        Deactivate
                      </button>
                    </form>
                  ) : (
                    <form action={reactivateDoctorAction} className="inline">
                      <input type="hidden" name="id" value={d.id} />
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
