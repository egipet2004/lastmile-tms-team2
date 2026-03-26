import type { Metadata } from "next";

import { DashboardOverviewClient } from "@/components/dashboard/dashboard-overview";
import { auth } from "@/lib/auth";

export const metadata: Metadata = {
  title: "Dashboard - Last Mile TMS",
  description: "Operations dashboard for fleet, dispatch, coverage, and depots.",
};

export default async function DashboardPage() {
  const session = await auth();
  const displayName = session?.user?.name ?? session?.user?.email ?? "Operator";

  return (
    <DashboardOverviewClient
      accessToken={session?.accessToken ?? ""}
      displayName={displayName}
      isAdmin={session?.user.roles.includes("Admin") ?? false}
    />
  );
}
