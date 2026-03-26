import RouteDetailPage from "@/components/routes/route-detail-page";

export default function Page(props: { params: Promise<{ id: string }> }) {
  return <RouteDetailPage {...props} />;
}
