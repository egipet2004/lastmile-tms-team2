import type { SelectOption } from "@/types/forms";
import type { Depot, DepotOption } from "@/types/depots";

export function depotSelectOptions(
  depots: Array<Depot | DepotOption> | undefined,
): SelectOption<string>[] {
  if (!depots?.length) return [];
  return depots.map((d) => ({
    value: d.id,
    label: d.name,
  }));
}
