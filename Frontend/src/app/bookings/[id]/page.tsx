import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { requireAuth } from "@/lib/auth";
import { getAppointmentById } from "@/services/appointmentService";
import CancelAppointmentButton from "@/components/CancelAppointmentButton";
import RescheduleForm from "@/components/RescheduleForm";
import type { Role } from "@/types/auth";

export default async function BookingDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Patient") {
    redirect("/login");
  }

  const token = await requireAuth();
  const { id } = await params;
  const appointment = await getAppointmentById(Number(id), token);

  if (appointment === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load this appointment.</p>
        <Link href="/bookings" className="text-blue-900 underline">Back to bookings</Link>
      </div>
    );
  }

  const isFuture = new Date(appointment.appointmentDate).getTime() > Date.now();
  const canModify = appointment.status === "Scheduled" && isFuture;

  return (
    <div className="px-4 py-8 max-w-2xl mx-auto space-y-6">
      <Link href="/bookings" className="text-blue-900 underline text-sm">
        ← Back to bookings
      </Link>

      <div className="bg-white p-6 rounded shadow space-y-2">
        <h1 className="text-2xl font-semibold text-blue-900">Appointment details</h1>
        <p><span className="font-semibold">Status:</span> {appointment.status}</p>
        <p><span className="font-semibold">Date:</span> {new Date(appointment.appointmentDate).toLocaleString()}</p>
        <p><span className="font-semibold">Duration:</span> {appointment.durationInMin} min</p>
        <p><span className="font-semibold">Doctor:</span> {appointment.doctorFirstName} {appointment.doctorLastName}</p>
        <p><span className="font-semibold">Clinic:</span> {appointment.clinicName}</p>
        <p><span className="font-semibold">Type:</span> {appointment.appointmentCategoryName}</p>
      </div>

      {canModify && (
        <>
          <RescheduleForm
            appointmentId={appointment.id}
            currentDate={appointment.appointmentDate}
            currentDuration={appointment.durationInMin}
          />
          <CancelAppointmentButton appointmentId={appointment.id} />
        </>
      )}
    </div>
  );
}
