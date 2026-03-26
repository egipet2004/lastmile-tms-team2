const ROUTE_FIELDS = `
  id
  vehicleId
  vehiclePlate
  driverId
  driverName
  startDate
  endDate
  startMileage
  endMileage
  totalMileage
  status
  parcelCount
  parcelsDelivered
  createdAt
`;

export const PAGINATED_ROUTES = `
  query GetRoutes($vehicleId: UUID, $status: RouteStatus, $page: Int, $pageSize: Int) {
    routes(vehicleId: $vehicleId, status: $status, page: $page, pageSize: $pageSize) {
      items { ${ROUTE_FIELDS} }
      totalCount
      page
      pageSize
      totalPages
    }
  }
`;

export const ROUTE_BY_ID = `
  query GetRoute($id: UUID!) {
    route(id: $id) {
      ${ROUTE_FIELDS}
    }
  }
`;

export const VEHICLE_ROUTE_HISTORY = `
  query GetVehicleHistory($vehicleId: UUID!, $status: RouteStatus, $page: Int, $pageSize: Int) {
    vehicleHistory(vehicleId: $vehicleId, status: $status, page: $page, pageSize: $pageSize) {
      items { ${ROUTE_FIELDS} }
      totalCount
      page
      pageSize
      totalPages
    }
  }
`;

export const CREATE_ROUTE = `
  mutation CreateRoute($input: CreateRouteDtoInput!) {
    createRoute(input: $input) {
      ${ROUTE_FIELDS}
    }
  }
`;
