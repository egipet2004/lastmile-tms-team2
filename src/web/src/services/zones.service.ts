import {
  CREATE_ZONE,
  DELETE_ZONE,
  UPDATE_ZONE,
  ZONE_BY_ID,
  ZONES_LIST,
} from "@/graphql/zones";
import { graphqlRequest } from "@/lib/network/graphql-client";
import type { CreateZoneRequest, UpdateZoneRequest, Zone } from "@/types/zones";

export const zonesService = {
  list: async (): Promise<Zone[]> => {
    const data = await graphqlRequest<{ zones: Zone[] }>(ZONES_LIST);
    return data.zones;
  },

  getById: async (id: string): Promise<Zone> => {
    const data = await graphqlRequest<{ zone: Zone | null }>(ZONE_BY_ID, { id });
    if (!data.zone) {
      throw new Error("Zone not found");
    }
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

    const data = await graphqlRequest<{ createZone: Zone }>(CREATE_ZONE, {
      input,
    });
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

    const data = await graphqlRequest<{ updateZone: Zone | null }>(UPDATE_ZONE, {
      id,
      input,
    });
    if (!data.updateZone) {
      throw new Error("Zone not found");
    }
    return data.updateZone;
  },

  delete: async (id: string): Promise<void> => {
    await graphqlRequest<{ deleteZone: boolean }>(DELETE_ZONE, { id });
  },
};
