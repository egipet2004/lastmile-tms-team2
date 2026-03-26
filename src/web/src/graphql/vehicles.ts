const VEHICLE_FIELDS = `
  id
  registrationPlate
  type
  parcelCapacity
  weightCapacity
  status
  depotId
  depotName
  totalRoutes
  routesCompleted
  totalMileage
  createdAt
  lastModifiedAt
`;

export const PAGINATED_VEHICLES = `
  query GetVehicles($page: Int, $pageSize: Int, $status: VehicleStatus, $depotId: UUID) {
    vehicles(page: $page, pageSize: $pageSize, status: $status, depotId: $depotId) {
      items { ${VEHICLE_FIELDS} }
      totalCount
      page
      pageSize
      totalPages
    }
  }
`;

export const VEHICLE_BY_ID = `
  query GetVehicle($id: UUID!) {
    vehicle(id: $id) {
      ${VEHICLE_FIELDS}
    }
  }
`;

export const CREATE_VEHICLE = `
  mutation CreateVehicle($input: CreateVehicleDtoInput!) {
    createVehicle(input: $input) {
      ${VEHICLE_FIELDS}
    }
  }
`;

export const UPDATE_VEHICLE = `
  mutation UpdateVehicle($id: UUID!, $input: UpdateVehicleDtoInput!) {
    updateVehicle(id: $id, input: $input) {
      ${VEHICLE_FIELDS}
    }
  }
`;

export const DELETE_VEHICLE = `
  mutation DeleteVehicle($id: UUID!) {
    deleteVehicle(id: $id)
  }
`;
