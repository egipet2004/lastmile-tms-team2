export const DRIVERS_LIST = `
  query GetDrivers($depotId: UUID) {
    drivers(depotId: $depotId) {
      id
      displayName
    }
  }
`;
