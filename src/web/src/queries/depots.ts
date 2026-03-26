import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { depotsService } from "@/services/depots.service";
import type { CreateDepotRequest, UpdateDepotRequest } from "@/types/depots";

export const depotKeys = {
  all: ["depots"] as const,
  list: () => [...depotKeys.all, "list"] as const,
  detail: (id: string) => [...depotKeys.all, "detail", id] as const,
};

export function useDepots() {
  const { status } = useSession();
  return useQuery({
    queryKey: depotKeys.list(),
    queryFn: () => depotsService.list(),
    enabled: status === "authenticated",
  });
}

export function useDepot(id: string) {
  const { status } = useSession();
  return useQuery({
    queryKey: depotKeys.detail(id),
    queryFn: () => depotsService.getById(id),
    enabled: status === "authenticated" && !!id,
  });
}

export function useCreateDepot() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateDepotRequest) => depotsService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: depotKeys.all });
    },
  });
}

export function useUpdateDepot() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateDepotRequest }) =>
      depotsService.update(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: depotKeys.all });
      queryClient.invalidateQueries({ queryKey: depotKeys.detail(id) });
    },
  });
}

export function useDeleteDepot() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) => depotsService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: depotKeys.all });
    },
  });
}
