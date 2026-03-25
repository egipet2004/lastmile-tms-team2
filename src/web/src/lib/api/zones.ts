import { graphqlRequest } from "@/lib/graphql";
import type { CreateZoneRequest, UpdateZoneRequest, Zone } from "@/types/zone";

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

export const zonesApi = {
  list: async (): Promise<Zone[]> => {
    const data = await graphqlRequest<{ zones: Zone[] }>(`
      query GetZones { zones { ${ZONE_FIELDS} } }
    `);
    return data.zones;
  },

  get: async (id: string): Promise<Zone> => {
    const data = await graphqlRequest<{ zone: Zone | null }>(`
      query GetZone($id: UUID!) { zone(id: $id) { ${ZONE_FIELDS} } }
    `, { id });
    if (!data.zone) throw new Error("Zone not found");
    return data.zone;
  },

  create: async (req: CreateZoneRequest): Promise<Zone> => {
    const input: Record<string, unknown> = {
      name: req.name,
      depotId: req.depotId,
      isActive: req.isActive,
    };
    if (req.boundaryInput.geoJson) {
      input.geoJson = req.boundaryInput.geoJson;
    } else if (req.boundaryInput.coordinates) {
      input.coordinates = req.boundaryInput.coordinates;
    } else if (req.boundaryInput.boundaryWkt) {
      input.boundaryWkt = req.boundaryInput.boundaryWkt;
    }
    const data = await graphqlRequest<{ createZone: Zone }>(`
      mutation CreateZone($input: CreateZoneInput!) {
        createZone(input: $input) { ${ZONE_FIELDS} }
      }
    `, { input });
    return data.createZone;
  },

  update: async (id: string, req: UpdateZoneRequest): Promise<Zone> => {
    const input: Record<string, unknown> = {
      name: req.name,
      depotId: req.depotId,
      isActive: req.isActive,
    };
    if (req.boundaryInput.geoJson) {
      input.geoJson = req.boundaryInput.geoJson;
    } else if (req.boundaryInput.coordinates) {
      input.coordinates = req.boundaryInput.coordinates;
    } else if (req.boundaryInput.boundaryWkt) {
      input.boundaryWkt = req.boundaryInput.boundaryWkt;
    }
    const data = await graphqlRequest<{ updateZone: Zone | null }>(`
      mutation UpdateZone($id: UUID!, $input: UpdateZoneInput!) {
        updateZone(id: $id, input: $input) { ${ZONE_FIELDS} }
      }
    `, { id, input });
    if (!data.updateZone) throw new Error("Zone not found");
    return data.updateZone;
  },

  delete: async (id: string): Promise<void> => {
    await graphqlRequest<{ deleteZone: boolean }>(`
      mutation DeleteZone($id: UUID!) { deleteZone(id: $id) }
    `, { id });
  },
};
