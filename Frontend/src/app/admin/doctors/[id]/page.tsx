import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";
import Link from "next/link";
import { requireAuth } from "@/lib/auth";
import { getDoctorByIdIncludingInactive } from "@/services/doctorService";
import { getAllClinics } from "@/services/clinicService";
import { getAllSpecialities } from "@/services/specialityService";
import EditDoctorForm from "@/components/EditDoctorForm";
import type { Role } from "@/types/auth";

export default async function EditDoctorPage({
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

  const [doctor, clinics, specialities] = await Promise.all([
    getDoctorByIdIncludingInactive(Number(id), token),
    getAllClinics(),
    getAllSpecialities(),
  ]);

  if (doctor === null) {
    notFound();
  }

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
        <EditDoctorForm initialData={doctor} clinics={clinics} specialities={specialities} />
      </div>
    </div>
  );
}
