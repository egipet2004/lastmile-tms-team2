const DEPOT_ADDRESS_FIELDS = `
  street1
  street2
  city
  state
  postalCode
  countryCode
  isResidential
  contactName
  companyName
  phone
  email
`;

const DEPOT_HOURS_FIELDS = `
  dayOfWeek
  openTime
  closedTime
  isClosed
`;

const DEPOT_FIELDS = `
  id
  name
  address { ${DEPOT_ADDRESS_FIELDS} }
  operatingHours { ${DEPOT_HOURS_FIELDS} }
  isActive
  createdAt
  updatedAt
`;

export const DEPOTS_LIST = `
  query GetDepots {
    depots {
      ${DEPOT_FIELDS}
    }
  }
`;

export const DEPOT_BY_ID = `
  query GetDepot($id: UUID!) {
    depot(id: $id) {
      ${DEPOT_FIELDS}
    }
  }
`;

export const CREATE_DEPOT = `
  mutation CreateDepot($input: CreateDepotInput!) {
    createDepot(input: $input) {
      ${DEPOT_FIELDS}
    }
  }
`;

export const UPDATE_DEPOT = `
  mutation UpdateDepot($id: UUID!, $input: UpdateDepotInput!) {
    updateDepot(id: $id, input: $input) {
      ${DEPOT_FIELDS}
    }
  }
`;

export const DELETE_DEPOT = `
  mutation DeleteDepot($id: UUID!) {
    deleteDepot(id: $id)
  }
`;
