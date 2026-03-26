import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { zonesService } from "@/services/zones.service";
import type { CreateZoneRequest, UpdateZoneRequest } from "@/types/zones";

export const zoneKeys = {
  all: ["zones"] as const,
  list: () => [...zoneKeys.all, "list"] as const,
  detail: (id: string) => [...zoneKeys.all, "detail", id] as const,
};

export function useZones() {
  const { status } = useSession();
  return useQuery({
    queryKey: zoneKeys.list(),
    queryFn: () => zonesService.list(),
    enabled: status === "authenticated",
  });
}

export function useZone(id: string) {
  const { status } = useSession();
  return useQuery({
    queryKey: zoneKeys.detail(id),
    queryFn: () => zonesService.getById(id),
    enabled: status === "authenticated" && !!id,
  });
}

export function useCreateZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateZoneRequest) => zonesService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: zoneKeys.all });
    },
  });
}

export function useUpdateZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateZoneRequest }) =>
      zonesService.update(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: zoneKeys.all });
      queryClient.invalidateQueries({ queryKey: zoneKeys.detail(id) });
    },
  });
}

export function useDeleteZone() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => zonesService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: zoneKeys.all });
    },
  });
}
