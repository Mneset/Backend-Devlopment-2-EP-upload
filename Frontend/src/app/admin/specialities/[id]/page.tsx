import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";
import Link from "next/link";
import { getSpecialityById } from "@/services/specialityService";
import EditSpecialityForm from "@/components/EditSpecialityForm";
import type { Role } from "@/types/auth";

export default async function EditSpecialityPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  const { id } = await params;
  const speciality = await getSpecialityById(Number(id));

  if (speciality === null) {
    notFound();
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/specialities" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to specialities
      </Link>
      <div className="w-full max-w-md">
        <EditSpecialityForm initialData={speciality} />
      </div>
    </div>
  );
}
