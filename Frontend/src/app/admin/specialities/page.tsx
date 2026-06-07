import Link from "next/link";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { getAllSpecialities } from "@/services/specialityService";
import { deleteSpecialityAction } from "@/modules/specialities/actions";
import type { Role } from "@/types/auth";

export default async function AdminSpecialitiesPage() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;

  if (role !== "Admin") {
    redirect("/login");
  }

  const specialities = await getAllSpecialities();

  if (specialities === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load specialities.</p>
      </div>
    );
  }

  return (
    <div className="px-4 py-8 max-w-4xl mx-auto">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-semibold text-blue-900">Specialities</h1>
        <Link
          href="/admin/specialities/new"
          className="bg-blue-900 text-white px-4 py-2 rounded hover:bg-blue-950"
        >
          New speciality
        </Link>
      </div>

      {specialities.length === 0 ? (
        <p className="text-gray-600">No specialities yet.</p>
      ) : (
        <table className="w-full bg-white shadow rounded overflow-hidden">
          <thead className="bg-blue-900 text-white text-left">
            <tr>
              <th className="p-3">Name</th>
              <th className="p-3 text-right">Actions</th>
            </tr>
          </thead>
          <tbody>
            {specialities.map((s) => (
              <tr key={s.id} className="border-t">
                <td className="p-3">{s.name}</td>
                <td className="p-3 text-right space-x-2">
                  <Link
                    href={`/admin/specialities/${s.id}`}
                    className="text-blue-900 underline"
                  >
                    Edit
                  </Link>
                  <form action={deleteSpecialityAction} className="inline">
                    <input type="hidden" name="id" value={s.id} />
                    <button type="submit" className="text-red-700 underline">
                      Delete
                    </button>
                  </form>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
