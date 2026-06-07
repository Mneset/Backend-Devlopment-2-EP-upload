import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";
import { getAllClinics } from "@/services/clinicService";
import { getAllSpecialities } from "@/services/specialityService";
import CreateDoctorForm from "@/components/CreateDoctorForm";
import type { Role } from "@/types/auth";

export default async function NewDoctorPage() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  const [clinics, specialities] = await Promise.all([
    getAllClinics(),
    getAllSpecialities(),
  ]);

  if (clinics === null || specialities === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load form options.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/doctors" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to doctors
      </Link>
      <div className="w-full max-w-md">
        <CreateDoctorForm clinics={clinics} specialities={specialities} />
      </div>
    </div>
  );
}
