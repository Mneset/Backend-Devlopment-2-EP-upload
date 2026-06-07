import { requireAuth } from "@/lib/auth";
import { getMyProfile } from "@/services/patientService";
import DeactivateButton from "@/components/DeactivateButton";
import ProfileForm from "./ProfileForm";

export default async function ProfilePage() {
  const token = await requireAuth();
  const profile = await getMyProfile(token);

  if (profile === null) {
    return (
      <div className="flex justify-center px-4 py-8">
        <p className="text-red-600">Could not load your profile.</p>
      </div>
    );
  }

  return (
    <div className="flex justify-center px-4 py-8">
      <div className="w-full max-w-md space-y-6">
        <ProfileForm initialData={profile} />

        <div className="bg-white p-6 rounded shadow space-y-3">
          <h2 className="text-xl font-semibold text-red-700">Deactivate account</h2>
          <p className="text-sm text-gray-600">
            Your scheduled appointments will be cancelled and you will be logged out.
            An admin can reactivate your account if needed.
          </p>
          <DeactivateButton />
        </div>
      </div>
    </div>
  );
}
