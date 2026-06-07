import Link from "next/link";
import { cookies } from "next/headers";
import { logoutAction } from "@/modules/auth/actions";
import type { Role } from "@/types/auth";

export default async function NavLinks() {
  const cookieStore = await cookies();
  const role = cookieStore.get("role")?.value as Role | undefined;
  const isLoggedIn = role !== undefined;

  return (
    <>
      <Link href="/">Home</Link>

      {!isLoggedIn && (
        <>
          <Link href="/login">Log in</Link>
          <Link href="/register">Register</Link>
        </>
      )}

      {role === "Patient" && (
        <>
          <Link href="/bookings">My bookings</Link>
          <Link href="/profile">Profile</Link>
        </>
      )}

      {role === "Admin" && <Link href="/admin">Admin</Link>}

      {isLoggedIn && (
        <form action={logoutAction} className="ml-auto">
          <button type="submit" className="hover:underline">
            Log out
          </button>
        </form>
      )}
    </>
  );
}
