import { cookies } from "next/headers";
import DashboardCard from "@/components/DashboardCard";
import type { Role } from "@/types/auth";

export default async function Home() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;
  const isLoggedIn = role !== undefined;
  const isPatient = role === "Patient";

  return (
    <div className="flex flex-col items-center px-4 py-12 space-y-8">
      <h1 className="text-3xl font-semibold text-blue-900">Welcome to ClinicAPI</h1>

      <div className="flex flex-wrap justify-center gap-6 max-w-3xl w-full">
        <DashboardCard
          href="/book"
          title="Book an appointment"
          description="Browse clinics and doctors and book an Appointment."
        />

        {isPatient && (
          <>
            <DashboardCard
              href="/bookings"
              title="My bookings"
              description="View your upcoming and past appointments."
            />
            <DashboardCard
              href="/profile"
              title="Profile"
              description="View and edit your account details."
            />
          </>
        )}

        {!isLoggedIn && (
          <>
            <DashboardCard
              href="/login"
              title="Log in"
              description="Already have an account? Sign in to manage your appointments."
            />
            <DashboardCard
              href="/register"
              title="Register"
              description="Create an account to track your bookings."
            />
          </>
        )}
      </div>
    </div>
  );
}
