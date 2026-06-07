import { getAllDoctors } from "@/services/doctorService";
import DoctorSearch from "@/components/DoctorSearch";

export default async function SearchPage() {
  const doctors = await getAllDoctors();

  if (doctors === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load doctors.</p>
      </div>
    );
  }

  return (
    <div className="px-4 py-8 max-w-2xl mx-auto space-y-6">
      <h1 className="text-2xl font-semibold text-blue-900">Find a doctor</h1>
      <DoctorSearch doctors={doctors} />
    </div>
  );
}
