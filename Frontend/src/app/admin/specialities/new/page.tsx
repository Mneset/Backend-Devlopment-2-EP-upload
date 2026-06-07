import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import Link from "next/link";
import CreateSpecialityForm from "@/components/CreateSpecialityForm";
import type { Role } from "@/types/auth";

export default async function NewSpecialityPage() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  return (
    <div className="flex flex-col items-center px-4 py-8 space-y-4">
      <Link href="/admin/specialities" className="text-blue-900 underline text-sm self-start max-w-md w-full">
        ← Back to specialities
      </Link>
      <div className="w-full max-w-md">
        <CreateSpecialityForm />
      </div>
    </div>
  );
}
