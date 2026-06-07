import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";
import Link from "next/link";
import { requireAuth } from "@/lib/auth";
import { getClinicByIdIncludingInactive } from "@/services/clinicService";
import EditClinicForm from "@/components/EditClinicForm";
import type { Role } from "@/types/auth";

export default async function EditClinicPage({
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
  const clinic = await getClinicByIdIncludingInactive(Number(id), token);

  if (clinic === null) {
    notFound();
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/clinics" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to clinics
      </Link>
      <div className="w-full max-w-md">
        <EditClinicForm initialData={clinic} />
      </div>
    </div>
  );
}
