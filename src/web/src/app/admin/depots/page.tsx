"use client";

import { useState } from "react";
import { useSession } from "next-auth/react";
import { Plus, Pencil, Trash2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { useDepots, useCreateDepot, useUpdateDepot, useDeleteDepot } from "@/lib/hooks/useDepots";
import type { Depot, CreateDepotRequest } from "@/types/depots";

const DAYS = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

function DepotRow({ depot, onEdit, onDelete }: { depot: Depot; onEdit: (d: Depot) => void; onDelete: (d: Depot) => void }) {
  const city = depot.address?.city ?? "—";
  const country = depot.address?.countryCode ?? "";
  const hours = depot.operatingHours;
  const activeHours = hours?.filter((h) => !h.isClosed) ?? [];

  return (
    <tr className="border-b border-border/50 hover:bg-muted/30 transition-colors">
      <td className="px-4 py-3 font-medium">{depot.name}</td>
      <td className="px-4 py-3 text-sm text-muted-foreground">
        {[city, country].filter(Boolean).join(", ") || "—"}
      </td>
      <td className="px-4 py-3 text-sm text-muted-foreground">
        {activeHours.length > 0
          ? `${activeHours.length} day${activeHours.length > 1 ? "s" : ""}`
          : hours?.some((h) => h.isClosed) ? "Closed" : "—"}
      </td>
      <td className="px-4 py-3">
        <span
          className={cn(
            "inline-flex rounded-full px-2 py-0.5 text-xs font-medium",
            depot.isActive
              ? "bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-200"
              : "bg-neutral-100 text-neutral-600 dark:bg-neutral-800 dark:text-neutral-400",
          )}
        >
          {depot.isActive ? "Active" : "Inactive"}
        </span>
      </td>
      <td className="px-4 py-3">
        <div className="flex items-center justify-end gap-1">
          <button
            type="button"
            onClick={() => onEdit(depot)}
            className="inline-flex items-center gap-1 rounded-lg px-3 py-1.5 text-sm text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
          >
            <Pencil className="size-3.5" aria-hidden />
            Edit
          </button>
          <button
            type="button"
            onClick={() => onDelete(depot)}
            className="inline-flex items-center gap-1 rounded-lg px-3 py-1.5 text-sm text-destructive/70 hover:bg-destructive/10 hover:text-destructive transition-colors"
          >
            <Trash2 className="size-3.5" aria-hidden />
            Delete
          </button>
        </div>
      </td>
    </tr>
  );
}

interface DepotFormData {
  name: string;
  street1: string;
  city: string;
  state: string;
  postalCode: string;
  countryCode: string;
  isActive: boolean;
}

interface HoursData {
  dayOfWeek: number;
  openTime: string;
  closedTime: string;
  isClosed: boolean;
}

function defaultForm(): { depot: DepotFormData; hours: HoursData[] } {
  return {
    depot: {
      name: "",
      street1: "",
      city: "",
      state: "",
      postalCode: "",
      countryCode: "AU",
      isActive: true,
    },
    hours: DAYS.map((_, i) => ({
      dayOfWeek: i,
      openTime: "08:00",
      closedTime: "17:00",
      isClosed: i === 0 || i === 6,
    })),
  };
}

function DepotForm({
  initial,
  onSubmit,
  onCancel,
  error,
  isPending,
}: {
  initial?: { depot: Depot; hours: HoursData[] };
  onSubmit: (data: CreateDepotRequest) => void;
  onCancel: () => void;
  error?: string;
  isPending: boolean;
}) {
  const [form, setForm] = useState<DepotFormData>(
    initial?.depot
      ? {
          name: initial.depot.name,
          street1: initial.depot.address?.street1 ?? "",
          city: initial.depot.address?.city ?? "",
          state: initial.depot.address?.state ?? "",
          postalCode: initial.depot.address?.postalCode ?? "",
          countryCode: initial.depot.address?.countryCode ?? "AU",
          isActive: initial.depot.isActive,
        }
      : defaultForm().depot,
  );
  const [hours, setHours] = useState<HoursData[]>(
    initial?.hours ?? defaultForm().hours,
  );

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const address = {
      street1: form.street1,
      city: form.city,
      state: form.state,
      postalCode: form.postalCode,
      countryCode: form.countryCode,
      isResidential: false,
    };
    const operatingHours = hours
      .filter((h) => !h.isClosed && h.openTime && h.closedTime)
      .map((h) => ({
        dayOfWeek: h.dayOfWeek,
        openTime: h.openTime,
        closedTime: h.closedTime,
        isClosed: h.isClosed,
      }));
    onSubmit({ name: form.name, address, operatingHours, isActive: form.isActive });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-2">
        <div className="sm:col-span-2">
          <Label htmlFor="d-name">Depot name</Label>
          <Input
            id="d-name"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
          />
        </div>
        <div className="sm:col-span-2">
          <Label htmlFor="d-street">Street address</Label>
          <Input
            id="d-street"
            value={form.street1}
            onChange={(e) => setForm({ ...form, street1: e.target.value })}
            required
          />
        </div>
        <div>
          <Label htmlFor="d-city">City</Label>
          <Input
            id="d-city"
            value={form.city}
            onChange={(e) => setForm({ ...form, city: e.target.value })}
            required
          />
        </div>
        <div>
          <Label htmlFor="d-state">State</Label>
          <Input
            id="d-state"
            value={form.state}
            onChange={(e) => setForm({ ...form, state: e.target.value })}
            required
          />
        </div>
        <div>
          <Label htmlFor="d-postal">Postal code</Label>
          <Input
            id="d-postal"
            value={form.postalCode}
            onChange={(e) => setForm({ ...form, postalCode: e.target.value })}
            required
          />
        </div>
        <div>
          <Label htmlFor="d-country">Country code</Label>
          <Input
            id="d-country"
            value={form.countryCode}
            maxLength={2}
            onChange={(e) =>
              setForm({ ...form, countryCode: e.target.value.toUpperCase().slice(0, 2) })
            }
            required
          />
        </div>
        <div>
          <Label htmlFor="d-active">Status</Label>
          <select
            id="d-active"
            value={form.isActive ? "true" : "false"}
            onChange={(e) => setForm({ ...form, isActive: e.target.value === "true" })}
            className="flex h-10 w-full items-center rounded-xl border border-input/90 bg-background px-3 py-2 text-sm"
          >
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </div>
      </div>

      <div>
        <p className="mb-2 text-sm font-medium">Operating hours</p>
        <div className="space-y-2">
          {hours.map((h, i) => (
            <div key={i} className="flex items-center gap-3 rounded-lg border border-border/60 bg-muted/20 px-3 py-2">
              <span className="w-24 shrink-0 text-sm font-medium">{DAYS[i]}</span>
              <label className="flex items-center gap-1.5 text-xs text-muted-foreground">
                <input
                  type="checkbox"
                  checked={h.isClosed}
                  onChange={(e) => {
                    const next = [...hours];
                    next[i] = { ...next[i], isClosed: e.target.checked };
                    setHours(next);
                  }}
                  className="accent-primary"
                />
                Closed
              </label>
              {!h.isClosed && (
                <div className="flex items-center gap-1">
                  <input
                    type="time"
                    value={h.openTime}
                    onChange={(e) => {
                      const next = [...hours];
                      next[i] = { ...next[i], openTime: e.target.value };
                      setHours(next);
                    }}
                    className="rounded-lg border border-input bg-background px-2 py-1 text-sm"
                  />
                  <span className="text-muted-foreground">–</span>
                  <input
                    type="time"
                    value={h.closedTime}
                    onChange={(e) => {
                      const next = [...hours];
                      next[i] = { ...next[i], closedTime: e.target.value };
                      setHours(next);
                    }}
                    className="rounded-lg border border-input bg-background px-2 py-1 text-sm"
                  />
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      {error && <p className="text-sm text-destructive">{error}</p>}

      <div className="flex justify-end gap-3 border-t border-border/50 pt-4">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" disabled={isPending}>
          {isPending
            ? "Saving…"
            : initial
              ? "Save changes"
              : "Create depot"}
        </Button>
      </div>
    </form>
  );
}

export default function AdminDepotsPage() {
  const { status } = useSession();
  const { data: depots, isLoading, error } = useDepots();
  const createDepot = useCreateDepot();
  const updateDepot = useUpdateDepot();
  const deleteDepot = useDeleteDepot();

  const [showCreate, setShowCreate] = useState(false);
  const [editTarget, setEditTarget] = useState<Depot | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Depot | null>(null);
  const [submitError, setSubmitError] = useState<string | undefined>();

  if (status === "loading" || isLoading) {
    return (
      <div className="space-y-4">
        <div className="h-10 w-48 animate-pulse rounded-lg bg-muted" />
        <div className="h-64 animate-pulse rounded-xl bg-muted/50" />
      </div>
    );
  }

  if (error) {
    return (
      <p className="text-sm text-destructive">
        Failed to load depots: {String(error)}
      </p>
    );
  }

  return (
    <>
      {/* Delete Confirmation Modal */}
      {deleteTarget && (
        <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/40 p-4 overflow-y-auto">
          <div className="w-full max-w-sm rounded-2xl border border-border bg-card p-6 shadow-xl mt-[15vh]">
            <h2 className="text-lg font-semibold mb-2">Delete depot</h2>
            <p className="text-sm text-muted-foreground mb-6">
              Are you sure you want to delete <strong>{deleteTarget.name}</strong>? This action cannot be undone.
            </p>
            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => { setDeleteTarget(null); setSubmitError(undefined); }}
              >
                Cancel
              </Button>
              <Button
                variant="destructive"
                disabled={deleteDepot.isPending}
                onClick={async () => {
                  try {
                    await deleteDepot.mutateAsync(deleteTarget.id);
                    setDeleteTarget(null);
                    setSubmitError(undefined);
                  } catch (e) {
                    setSubmitError(String(e));
                  }
                }}
              >
                {deleteDepot.isPending ? "Deleting…" : "Delete"}
              </Button>
            </div>
            {submitError && <p className="mt-3 text-sm text-destructive">{submitError}</p>}
          </div>
        </div>
      )}

      {/* Edit Modal */}
      {editTarget && (
        <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/40 p-4 overflow-y-auto">
          <div className="w-full max-w-2xl rounded-2xl border border-border bg-card p-6 shadow-xl">
            <div className="mb-6 flex items-center justify-between">
              <h2 className="text-lg font-semibold">Edit depot</h2>
              <button
                type="button"
                onClick={() => { setEditTarget(null); setSubmitError(undefined); }}
                className="rounded-lg p-1 hover:bg-muted transition-colors"
              >
                <X className="size-5" />
              </button>
            </div>
            <DepotForm
              initial={editTarget ? { depot: editTarget, hours: [] } : undefined}
              onSubmit={async (data) => {
                try {
                  await updateDepot.mutateAsync({ id: editTarget!.id, data });
                  setEditTarget(null);
                  setSubmitError(undefined);
                } catch (e) {
                  setSubmitError(String(e));
                }
              }}
              onCancel={() => { setEditTarget(null); setSubmitError(undefined); }}
              error={submitError}
              isPending={updateDepot.isPending}
            />
          </div>
        </div>
      )}

      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Depots</h1>
          <p className="text-sm text-muted-foreground">
            Manage logistics hubs and operating hours.
          </p>
        </div>
        <Button onClick={() => setShowCreate(true)} className="gap-2">
          <Plus className="size-4" aria-hidden />
          Add depot
        </Button>
      </div>

      {/* Create Form */}
      {showCreate && (
        <div className="mb-6 rounded-2xl border border-border bg-card p-6">
          <h2 className="mb-4 text-base font-semibold">New depot</h2>
          <DepotForm
            onSubmit={async (data) => {
              try {
                await createDepot.mutateAsync(data);
                setShowCreate(false);
                setSubmitError(undefined);
              } catch (e) {
                setSubmitError(String(e));
              }
            }}
            onCancel={() => { setShowCreate(false); setSubmitError(undefined); }}
            error={submitError}
            isPending={createDepot.isPending}
          />
        </div>
      )}

      {/* List */}
      {submitError && !showCreate && !editTarget && !deleteTarget && (
        <p className="mb-4 text-sm text-destructive">{submitError}</p>
      )}

      {depots && depots.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border p-12 text-center">
          <p className="font-medium">No depots yet</p>
          <p className="mt-1 text-sm text-muted-foreground">
            Click &ldquo;Add depot&rdquo; to create your first hub.
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-border">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border bg-muted/40">
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Name</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Location</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Hours</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Status</th>
                <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>
              </tr>
            </thead>
            <tbody>
              {depots?.map((d) => (
                <DepotRow
                  key={d.id}
                  depot={d}
                  onEdit={(depot) => setEditTarget(depot)}
                  onDelete={(depot) => setDeleteTarget(depot)}
                />
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}
