"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { depotsApi } from "@/lib/api/depots";
import type { CreateDepotRequest, UpdateDepotRequest } from "@/types/depots";

export function useDepots() {
  const { status } = useSession();
  return useQuery({
    queryKey: ["depots"],
    queryFn: () => depotsApi.list(),
    enabled: status === "authenticated",
  });
}

export function useDepot(id: string) {
  const { status } = useSession();
  return useQuery({
    queryKey: ["depot", id],
    queryFn: () => depotsApi.get(id),
    enabled: status === "authenticated" && !!id,
  });
}

export function useCreateDepot() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateDepotRequest) => depotsApi.create(data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["depots"] }),
  });
}

export function useUpdateDepot() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateDepotRequest }) =>
      depotsApi.update(id, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["depots"] }),
  });
}

export function useDeleteDepot() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => depotsApi.delete(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["depots"] }),
  });
}
