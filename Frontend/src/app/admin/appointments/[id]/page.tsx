import Link from "next/link";
import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAppointmentById } from "@/services/appointmentService";
import {
  adminCancelAppointmentAction,
  adminCompleteAppointmentAction,
} from "@/modules/appointments/actions";
import AdminRescheduleForm from "@/components/AdminRescheduleForm";
import type { Role } from "@/types/auth";

export default async function AdminAppointmentDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  const token = await requireAuth();
  const { id } = await params;
  const appointment = await getAppointmentById(Number(id), token);

  if (appointment === null) {
    notFound();
  }

  const isFuture = new Date(appointment.appointmentDate).getTime() > Date.now();
  const canModify = appointment.status === "Scheduled" && isFuture;

  return (
    <div className="px-4 py-8 max-w-2xl mx-auto space-y-6">
      <Link href="/admin/appointments" className="text-blue-900 underline text-sm">
        ← Back to appointments
      </Link>

      <div className="bg-white p-6 rounded shadow space-y-2">
        <h1 className="text-2xl font-semibold text-blue-900">Appointment details</h1>
        <p><span className="font-semibold">Status:</span> {appointment.status}</p>
        <p><span className="font-semibold">Date:</span> {new Date(appointment.appointmentDate).toLocaleString()}</p>
        <p><span className="font-semibold">Duration:</span> {appointment.durationInMin} min</p>
        <p><span className="font-semibold">Patient:</span> {appointment.patientFirstName} {appointment.patientLastName}</p>
        <p><span className="font-semibold">Doctor:</span> {appointment.doctorFirstName} {appointment.doctorLastName}</p>
        <p><span className="font-semibold">Clinic:</span> {appointment.clinicName}</p>
        <p><span className="font-semibold">Type:</span> {appointment.appointmentCategoryName}</p>
      </div>

      {canModify && (
        <>
          <AdminRescheduleForm
            appointmentId={appointment.id}
            currentDate={appointment.appointmentDate}
            currentDuration={appointment.durationInMin}
          />

          <div className="bg-white p-6 rounded shadow space-y-3">
            <h2 className="text-xl font-semibold text-blue-900">Actions</h2>
            <form action={adminCompleteAppointmentAction}>
              <input type="hidden" name="id" value={appointment.id} />
              <button
                type="submit"
                className="w-full bg-green-700 text-white py-2 rounded hover:bg-green-800"
              >
                Mark as completed
              </button>
            </form>
            <form action={adminCancelAppointmentAction}>
              <input type="hidden" name="id" value={appointment.id} />
              <button
                type="submit"
                className="w-full bg-red-700 text-white py-2 rounded hover:bg-red-800"
              >
                Cancel appointment
              </button>
            </form>
          </div>
        </>
      )}

      {appointment.status === "Scheduled" && !isFuture && (
        <div className="bg-white p-6 rounded shadow space-y-3">
          <h2 className="text-xl font-semibold text-blue-900">Actions</h2>
          <p className="text-sm text-gray-600">
            This appointment is in the past. Mark it as completed or cancel it.
          </p>
          <form action={adminCompleteAppointmentAction}>
            <input type="hidden" name="id" value={appointment.id} />
            <button
              type="submit"
              className="w-full bg-green-700 text-white py-2 rounded hover:bg-green-800"
            >
              Mark as completed
            </button>
          </form>
          <form action={adminCancelAppointmentAction}>
            <input type="hidden" name="id" value={appointment.id} />
            <button
              type="submit"
              className="w-full bg-red-700 text-white py-2 rounded hover:bg-red-800"
            >
              Cancel appointment
            </button>
          </form>
        </div>
      )}
    </div>
  );
}
