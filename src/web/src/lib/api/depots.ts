import { graphqlRequest } from "@/lib/graphql";
import type { CreateDepotRequest, Depot, UpdateDepotRequest } from "@/types/depots";

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

export const depotsApi = {
  list: async (): Promise<Depot[]> => {
    const data = await graphqlRequest<{ depots: Depot[] }>(`
      query GetDepots { depots { ${DEPOT_FIELDS} } }
    `);
    return data.depots;
  },

  get: async (id: string): Promise<Depot> => {
    const data = await graphqlRequest<{ depot: Depot | null }>(`
      query GetDepot($id: UUID!) { depot(id: $id) { ${DEPOT_FIELDS} } }
    `, { id });
    if (!data.depot) throw new Error("Depot not found");
    return data.depot;
  },

  create: async (req: CreateDepotRequest): Promise<Depot> => {
    const data = await graphqlRequest<{ createDepot: Depot }>(`
      mutation CreateDepot($input: CreateDepotInput!) {
        createDepot(input: $input) { ${DEPOT_FIELDS} }
      }
    `, {
      input: {
        name: req.name,
        address: {
          street1: req.address.street1,
          street2: req.address.street2 ?? null,
          city: req.address.city,
          state: req.address.state,
          postalCode: req.address.postalCode,
          countryCode: req.address.countryCode,
          isResidential: req.address.isResidential,
          contactName: req.address.contactName ?? null,
          companyName: req.address.companyName ?? null,
          phone: req.address.phone ?? null,
          email: req.address.email ?? null,
        },
        operatingHours: req.operatingHours ?? [],
        isActive: req.isActive,
      },
    });
    return data.createDepot;
  },

  update: async (id: string, req: UpdateDepotRequest): Promise<Depot> => {
    const input: Record<string, unknown> = {
      name: req.name,
      isActive: req.isActive,
    };
    if (req.address) {
      input.address = {
        street1: req.address.street1,
        street2: req.address.street2 ?? null,
        city: req.address.city,
        state: req.address.state,
        postalCode: req.address.postalCode,
        countryCode: req.address.countryCode,
        isResidential: req.address.isResidential,
        contactName: req.address.contactName ?? null,
        companyName: req.address.companyName ?? null,
        phone: req.address.phone ?? null,
        email: req.address.email ?? null,
      };
    }
    if (req.operatingHours) {
      input.operatingHours = req.operatingHours;
    }
    const data = await graphqlRequest<{ updateDepot: Depot | null }>(`
      mutation UpdateDepot($id: UUID!, $input: UpdateDepotInput!) {
        updateDepot(id: $id, input: $input) { ${DEPOT_FIELDS} }
      }
    `, { id, input });
    if (!data.updateDepot) throw new Error("Depot not found");
    return data.updateDepot;
  },

  delete: async (id: string): Promise<void> => {
    await graphqlRequest<{ deleteDepot: boolean }>(`
      mutation DeleteDepot($id: UUID!) { deleteDepot(id: $id) }
    `, { id });
  },
};
