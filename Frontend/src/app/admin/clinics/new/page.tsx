import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";
import CreateClinicForm from "@/components/CreateClinicForm";
import type { Role } from "@/types/auth";

export default async function NewClinicPage() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/clinics" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to clinics
      </Link>
      <div className="w-full max-w-md">
        <CreateClinicForm />
      </div>
    </div>
  );
}
