import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getMyAppointments } from "@/services/appointmentService";
import type { Status } from "@/types/appointment";
import type { Role } from "@/types/auth";

const FILTERS: { label: string; value: Status | "All" }[] = [
  { label: "All", value: "All" },
  { label: "Scheduled", value: "Scheduled" },
  { label: "Completed", value: "Completed" },
  { label: "Cancelled", value: "Cancelled" },
];

export default async function BookingsPage({
  searchParams,
}: {
  searchParams: Promise<{ status?: string }>;
}) {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Patient") {
    redirect("/login");
  }

  const token = await requireAuth();
  const appointments = await getMyAppointments(token);
  const { status } = await searchParams;

  if (appointments === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load your bookings.</p>
      </div>
    );
  }

  const filtered =
    status === "Scheduled" || status === "Completed" || status === "Cancelled"
      ? appointments.filter((a) => a.status === status)
      : appointments;

  const now = Date.now();
  const nextAppointment = appointments
    .filter((a) => a.status === "Scheduled" && new Date(a.appointmentDate).getTime() > now)
    .sort(
      (a, b) =>
        new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime(),
    )[0];

  return (
    <div className="px-4 py-8 max-w-4xl mx-auto">
      <h1 className="text-2xl font-semibold text-blue-900 mb-4">My bookings</h1>

      <div className="bg-blue-50 border border-blue-900 rounded p-4 mb-6">
        <p className="text-sm uppercase tracking-wide text-blue-900 font-semibold mb-1">
          Next appointment
        </p>
        {nextAppointment ? (
          <Link href={`/bookings/${nextAppointment.id}`} className="block hover:underline">
            <p className="text-lg text-blue-900">
              {new Date(nextAppointment.appointmentDate).toLocaleString()} ·{" "}
              {nextAppointment.doctorFirstName} {nextAppointment.doctorLastName}
            </p>
            <p className="text-sm text-gray-700">
              {nextAppointment.appointmentCategoryName} · {nextAppointment.clinicName}
            </p>
          </Link>
        ) : (
          <p className="text-sm text-gray-700">You have no upcoming appointments.</p>
        )}
      </div>

      <div className="flex gap-2 mb-4">
        {FILTERS.map((f) => {
          const isActive = (status ?? "All") === f.value;
          const href = f.value === "All" ? "/bookings" : `/bookings?status=${f.value}`;
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
        <p className="text-gray-600">No bookings to show.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">Date</th>
              <th className="p-3">Doctor</th>
              <th className="p-3">Clinic</th>
              <th className="p-3">Type</th>
              <th className="p-3">Status</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((a) => (
              <tr key={a.id} className="border-t hover:bg-gray-50">
                <td className="p-3">
                  <Link href={`/bookings/${a.id}`} className="block">
                    {new Date(a.appointmentDate).toLocaleString()}
                  </Link>
                </td>
                <td className="p-3">{a.doctorFirstName} {a.doctorLastName}</td>
                <td className="p-3">{a.clinicName}</td>
                <td className="p-3">{a.appointmentCategoryName}</td>
                <td className="p-3">{a.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
