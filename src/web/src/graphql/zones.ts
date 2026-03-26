const ZONE_FIELDS = `
  id
  name
  boundary
  isActive
  depotId
  depotName
  createdAt
  updatedAt
`;

export const ZONES_LIST = `
  query GetZones {
    zones {
      ${ZONE_FIELDS}
    }
  }
`;

export const ZONE_BY_ID = `
  query GetZone($id: UUID!) {
    zone(id: $id) {
      ${ZONE_FIELDS}
    }
  }
`;

export const CREATE_ZONE = `
  mutation CreateZone($input: CreateZoneInput!) {
    createZone(input: $input) {
      ${ZONE_FIELDS}
    }
  }
`;

export const UPDATE_ZONE = `
  mutation UpdateZone($id: UUID!, $input: UpdateZoneInput!) {
    updateZone(id: $id, input: $input) {
      ${ZONE_FIELDS}
    }
  }
`;

export const DELETE_ZONE = `
  mutation DeleteZone($id: UUID!) {
    deleteZone(id: $id)
  }
`;
