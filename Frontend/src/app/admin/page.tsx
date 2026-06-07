import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import DashboardCard from "@/components/DashboardCard";
import type { Role } from "@/types/auth";

export default async function AdminDashboardPage() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  return (
    <div className="flex flex-col items-center px-4 py-12 space-y-8">
      <h1 className="text-3xl font-semibold text-blue-900">Admin dashboard</h1>

      <div className="flex flex-wrap justify-center gap-6 max-w-3xl w-full">
        <DashboardCard
          href="/admin/clinics"
          title="Clinics"
          description="Add, edit, and deactivate clinics."
        />
        <DashboardCard
          href="/admin/patients"
          title="Patients"
          description="View and manage all patients."
        />
        <DashboardCard
          href="/admin/doctors"
          title="Doctors"
          description="Add, edit, and deactivate doctors."
        />
        <DashboardCard
          href="/admin/specialities"
          title="Specialities"
          description="Manage the available medical specialities."
        />
        <DashboardCard
          href="/admin/appointment-categories"
          title="Appointment categories"
          description="Manage the available appointment types."
        />
        <DashboardCard
          href="/admin/appointments"
          title="Appointments"
          description="View and manage all appointments."
        />
      </div>
    </div>
  );
}
