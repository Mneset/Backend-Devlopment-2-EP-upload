import { cookies } from "next/headers";
import { getAllClinics } from "@/services/clinicService";
import { getAllDoctors } from "@/services/doctorService";
import { getAllAppointmentCategories } from "@/services/appointmentCategoryService";
import BookAppointmentForm from "@/components/BookAppointmentForm";
import GuestBookAppointmentForm from "@/components/GuestBookAppointmentForm";

export default async function BookPage() {
  const cookieStore = await cookies();
  const token = cookieStore.get("token")?.value;
  const isLoggedIn = token !== undefined;

  const [clinics, doctors, categories] = await Promise.all([
    getAllClinics(),
    getAllDoctors(),
    getAllAppointmentCategories(),
  ]);

  if (clinics === null || doctors === null || categories === null) {
    return (
      <div className="px-4 py-8 text-center">
        <p className="text-red-600">Could not load booking options.</p>
      </div>
    );
  }

  return (
    <div className="flex justify-center px-4 py-8">
      <div className="w-full max-w-md">
        {isLoggedIn ? (
          <BookAppointmentForm clinics={clinics} doctors={doctors} categories={categories} />
        ) : (
          <GuestBookAppointmentForm clinics={clinics} doctors={doctors} categories={categories} />
        )}
      </div>
    </div>
  );
}
