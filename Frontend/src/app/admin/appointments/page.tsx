import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAllAppointments } from "@/services/appointmentService";
import type { Role } from "@/types/auth";
import type { Status } from "@/types/appointment";

const FILTERS: { label: string; value: "all" | Status }[] = [
  { label: "All", value: "all" },
  { label: "Scheduled", value: "Scheduled" },
  { label: "Completed", value: "Completed" },
  { label: "Cancelled", value: "Cancelled" },
];

export default async function AdminAppointmentsPage({
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
  const { status } = await searchParams;
  const statusFilter =
    status === "Scheduled" || status === "Completed" || status === "Cancelled"
      ? (status as Status)
      : undefined;

  const appointments = await getAllAppointments(token, statusFilter);

  if (appointments === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load appointments.</p>
      </div>
    );
  }

  const sorted = [...appointments].sort(
    (a, b) => new Date(b.appointmentDate).getTime() - new Date(a.appointmentDate).getTime(),
  );

  return (
    <div className="px-4 py-8 max-w-6xl mx-auto">
      <h1 className="text-2xl font-semibold text-blue-900 mb-4">Appointments</h1>

      <div className="flex gap-2 mb-4">
        {FILTERS.map((f) => {
          const isActive = (status ?? "all") === f.value;
          const href = f.value === "all" ? "/admin/appointments" : `/admin/appointments?status=${f.value}`;
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

      {sorted.length === 0 ? (
        <p className="text-gray-600">No appointments to show.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">When</th>
              <th className="p-3">Patient</th>
              <th className="p-3">Doctor</th>
              <th className="p-3">Clinic</th>
              <th className="p-3">Type</th>
              <th className="p-3">Status</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {sorted.map((a) => (
              <tr key={a.id} className="border-t">
                <td className="p-3">{new Date(a.appointmentDate).toLocaleString()}</td>
                <td className="p-3">{a.patientFirstName} {a.patientLastName}</td>
                <td className="p-3">{a.doctorFirstName} {a.doctorLastName}</td>
                <td className="p-3">{a.clinicName}</td>
                <td className="p-3">{a.appointmentCategoryName}</td>
                <td className="p-3">
                  {a.status === "Scheduled" && <span className="text-blue-900">Scheduled</span>}
                  {a.status === "Completed" && <span className="text-green-700">Completed</span>}
                  {a.status === "Cancelled" && <span className="text-gray-500">Cancelled</span>}
                </td>
                <td className="p-3 text-right">
                  <Link href={`/admin/appointments/${a.id}`} className="text-blue-900 underline">
                    View
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
