import Link from "next/link";
import { cookies } from "next/headers";
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
        <Link
          href="/book"
          className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
        >
          <h2 className="text-xl font-semibold text-blue-900 mb-2">
            Book an appointment
          </h2>
          <p className="text-gray-600">
            Browse clinics and doctors and book an Appointment.
          </p>
        </Link>

        {isPatient && (
          <>
            <Link
              href="/bookings"
              className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
            >
              <h2 className="text-xl font-semibold text-blue-900 mb-2">
                My bookings
              </h2>
              <p className="text-gray-600">
                View your upcoming and past appointments.
              </p>
            </Link>

            <Link
              href="/profile"
              className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
            >
              <h2 className="text-xl font-semibold text-blue-900 mb-2">
                Profile
              </h2>
              <p className="text-gray-600">
                View and edit your account details.
              </p>
            </Link>
          </>
        )}

        {!isLoggedIn && (
          <>
            <Link
              href="/login"
              className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
            >
              <h2 className="text-xl font-semibold text-blue-900 mb-2">
                Log in
              </h2>
              <p className="text-gray-600">
                Already have an account? Sign in to manage your appointments.
              </p>
            </Link>

            <Link
              href="/register"
              className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
            >
              <h2 className="text-xl font-semibold text-blue-900 mb-2">
                Register
              </h2>
              <p className="text-gray-600">
                Create an account to track your bookings.
              </p>
            </Link>
          </>
        )}
      </div>
    </div>
  );
}
