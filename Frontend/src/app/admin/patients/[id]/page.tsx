import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";
import Link from "next/link";
import { requireAuth } from "@/lib/auth";
import { getPatientByIdIncludingInactive } from "@/services/patientService";
import EditPatientForm from "@/components/EditPatientForm";
import type { Role } from "@/types/auth";

export default async function EditPatientPage({
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

  const patient = await getPatientByIdIncludingInactive(Number(id), token);

  if (patient === null || !patient.isRegistered) {
    notFound();
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/patients" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to patients
      </Link>
      <div className="w-full max-w-md">
        <EditPatientForm initialData={patient} />
      </div>
    </div>
  );
}
