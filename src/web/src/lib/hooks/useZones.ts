"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { zonesApi } from "@/lib/api/zones";
import type { CreateZoneRequest, UpdateZoneRequest } from "@/types/zone";

export function useZones() {
  const { status } = useSession();
  return useQuery({
    queryKey: ["zones"],
    queryFn: () => zonesApi.list(),
    enabled: status === "authenticated",
  });
}

export function useZone(id: string) {
  const { status } = useSession();
  return useQuery({
    queryKey: ["zone", id],
    queryFn: () => zonesApi.get(id),
    enabled: status === "authenticated" && !!id,
  });
}

export function useCreateZone() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateZoneRequest) => zonesApi.create(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["zones"] }),
  });
}

export function useUpdateZone() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateZoneRequest }) =>
      zonesApi.update(id, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["zones"] }),
  });
}

export function useDeleteZone() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => zonesApi.delete(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["zones"] }),
  });
}
