"use client";

import { FilterListbox, type FilterListboxOption } from "@/components/form/filter-listbox";
import { ROUTE_STATUS_LABELS, ROUTE_STATUS_ORDER } from "@/lib/labels/routes";
import { RouteStatus } from "@/types/routes";

const routeStatusFilterOptions: FilterListboxOption<RouteStatus>[] = [
  { value: undefined, label: "All Statuses" },
  ...ROUTE_STATUS_ORDER.map((status) => ({
    value: status,
    label: ROUTE_STATUS_LABELS[status],
  })),
];

interface RouteStatusFilterProps {
  value: RouteStatus | undefined;
  onChange: (value: RouteStatus | undefined) => void;
}

export function RouteStatusFilter({
  value,
  onChange,
}: RouteStatusFilterProps) {
  return (
    <FilterListbox<RouteStatus>
      value={value}
      onChange={onChange}
      options={routeStatusFilterOptions}
    />
  );
}
