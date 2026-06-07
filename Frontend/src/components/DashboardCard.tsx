import Link from "next/link";

type Props = {
  href: string;
  title: string;
  description: string;
};

export default function DashboardCard({ href, title, description }: Props) {
  return (
    <Link
      href={href}
      className="block bg-white p-6 rounded shadow hover:shadow-lg transition w-full md:w-80"
    >
      <h2 className="text-xl font-semibold text-blue-900 mb-2">{title}</h2>
      <p className="text-gray-600">{description}</p>
    </Link>
  );
}
