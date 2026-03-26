"use client";

import { FilterListbox, type FilterListboxOption } from "@/components/form/filter-listbox";
import { VEHICLE_STATUS_LABELS, VEHICLE_STATUS_ORDER } from "@/lib/labels/vehicles";
import { VehicleStatus } from "@/types/vehicles";

const vehicleStatusFilterOptions: FilterListboxOption<VehicleStatus>[] = [
  { value: undefined, label: "All Statuses" },
  ...VEHICLE_STATUS_ORDER.map((v) => ({
    value: v,
    label: VEHICLE_STATUS_LABELS[v],
  })),
];

interface VehicleStatusFilterProps {
  value: VehicleStatus | undefined;
  onChange: (value: VehicleStatus | undefined) => void;
}

export function VehicleStatusFilter({
  value,
  onChange,
}: VehicleStatusFilterProps) {
  return (
    <FilterListbox<VehicleStatus>
      value={value}
      onChange={onChange}
      options={vehicleStatusFilterOptions}
    />
  );
}
