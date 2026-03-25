"use client";

import { useState } from "react";
import { useSession } from "next-auth/react";
import { Plus, Pencil, Trash2, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { useZones, useCreateZone, useUpdateZone, useDeleteZone } from "@/lib/hooks/useZones";
import { useDepots } from "@/lib/hooks/useDepots";
import type { Zone, CreateZoneRequest, UpdateZoneRequest } from "@/types/zone";

type BoundaryMode = "wkt" | "geojson" | "coordinates";

interface ZoneFormData {
  name: string;
  depotId: string;
  isActive: boolean;
  boundaryMode: BoundaryMode;
  boundaryWkt: string;
  geoJson: string;
  coordinates: string;
}

function defaultForm(): ZoneFormData {
  return {
    name: "",
    depotId: "",
    isActive: true,
    boundaryMode: "wkt",
    boundaryWkt: "",
    geoJson: "",
    coordinates: "",
  };
}

function ZoneForm({
  initial,
  depots,
  onSubmit,
  onCancel,
  error,
  isPending,
}: {
  initial?: Zone;
  depots: { id: string; name: string }[];
  onSubmit: (data: CreateZoneRequest | UpdateZoneRequest) => void;
  onCancel: () => void;
  error?: string;
  isPending: boolean;
}) {
  const [form, setForm] = useState<ZoneFormData>(() => {
    if (initial) {
      return {
        name: initial.name,
        depotId: initial.depotId,
        isActive: initial.isActive,
        boundaryMode: "wkt",
        boundaryWkt: initial.boundary,
        geoJson: "",
        coordinates: "",
      };
    }
    return defaultForm();
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const boundaryInput = {
      ...(form.boundaryMode === "wkt" && form.boundaryWkt.trim()
        ? { boundaryWkt: form.boundaryWkt.trim() }
        : {}),
      ...(form.boundaryMode === "geojson" && form.geoJson.trim()
        ? { geoJson: form.geoJson.trim() }
        : {}),
      ...(form.boundaryMode === "coordinates" && form.coordinates.trim()
        ? {
            coordinates: form.coordinates
              .trim()
              .split(/\s*;\s*/)
              .map((p) => {
                const [lon, lat] = p.trim().split(/\s*,\s*/);
                return [parseFloat(lon), parseFloat(lat)] as [number, number];
              }),
          }
        : {}),
    };
    onSubmit({
      name: form.name,
      depotId: form.depotId,
      isActive: form.isActive,
      boundaryInput,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid gap-4 sm:grid-cols-2">
        <div>
          <Label htmlFor="z-name">Zone name</Label>
          <Input
            id="z-name"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
            required
          />
        </div>
        <div>
          <Label htmlFor="z-depot">Depot</Label>
          <select
            id="z-depot"
            value={form.depotId}
            onChange={(e) => setForm({ ...form, depotId: e.target.value })}
            required
            className="flex h-10 w-full items-center rounded-xl border border-input/90 bg-background px-3 py-2 text-sm"
          >
            <option value="">Select depot…</option>
            {depots.map((d) => (
              <option key={d.id} value={d.id}>{d.name}</option>
            ))}
          </select>
        </div>
        <div>
          <Label htmlFor="z-active">Status</Label>
          <select
            id="z-active"
            value={form.isActive ? "true" : "false"}
            onChange={(e) => setForm({ ...form, isActive: e.target.value === "true" })}
            className="flex h-10 w-full items-center rounded-xl border border-input/90 bg-background px-3 py-2 text-sm"
          >
            <option value="true">Active</option>
            <option value="false">Inactive</option>
          </select>
        </div>
      </div>

      {/* Boundary input */}
      <div>
        <p className="mb-2 text-sm font-medium">Zone boundary</p>
        <div className="flex gap-4 mb-3">
          {(["wkt", "geojson", "coordinates"] as BoundaryMode[]).map((mode) => (
            <label key={mode} className="flex items-center gap-1.5 text-sm cursor-pointer">
              <input
                type="radio"
                name="boundary-mode"
                checked={form.boundaryMode === mode}
                onChange={() => setForm({ ...form, boundaryMode: mode })}
                className="accent-primary"
              />
              {mode === "wkt" ? "WKT" : mode === "geojson" ? "GeoJSON" : "Coordinates"}
            </label>
          ))}
        </div>
        {form.boundaryMode === "wkt" && (
          <Input
            placeholder="POLYGON ((lon lat, lon lat, ...))"
            value={form.boundaryWkt}
            onChange={(e) => setForm({ ...form, boundaryWkt: e.target.value })}
            className="font-mono text-xs"
          />
        )}
        {form.boundaryMode === "geojson" && (
          <textarea
            placeholder='{"type":"Polygon","coordinates":[[[lon,lat],...]]}'
            value={form.geoJson}
            onChange={(e) => setForm({ ...form, geoJson: e.target.value })}
            rows={4}
            className="w-full rounded-xl border border-input/90 bg-background px-3 py-2 text-xs font-mono"
          />
        )}
        {form.boundaryMode === "coordinates" && (
          <Input
            placeholder="145.0, -37.8; 145.1, -37.8; 145.1, -37.7; 145.0, -37.7"
            value={form.coordinates}
            onChange={(e) => setForm({ ...form, coordinates: e.target.value })}
            className="font-mono text-xs"
          />
        )}
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
              : "Create zone"}
        </Button>
      </div>
    </form>
  );
}

export default function AdminZonesPage() {
  const { status } = useSession();
  const { data: zones, isLoading: zonesLoading, error: zonesError } = useZones();
  const { data: depots, isLoading: depotsLoading } = useDepots();
  const createZone = useCreateZone();
  const updateZone = useUpdateZone();
  const deleteZone = useDeleteZone();

  const [showCreate, setShowCreate] = useState(false);
  const [editTarget, setEditTarget] = useState<Zone | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Zone | null>(null);
  const [submitError, setSubmitError] = useState<string | undefined>();

  const isLoading = zonesLoading || depotsLoading;

  if (status === "loading" || isLoading) {
    return (
      <div className="space-y-4">
        <div className="h-10 w-48 animate-pulse rounded-lg bg-muted" />
        <div className="h-64 animate-pulse rounded-xl bg-muted/50" />
      </div>
    );
  }

  if (zonesError) {
    return (
      <p className="text-sm text-destructive">
        Failed to load zones: {String(zonesError)}
      </p>
    );
  }

  const depotMap = new Map((depots ?? []).map((d) => [d.id, d.name]));

  return (
    <>
      {/* Delete Confirmation Modal */}
      {deleteTarget && (
        <div className="fixed inset-0 z-50 flex items-start justify-center bg-black/40 p-4 overflow-y-auto">
          <div className="w-full max-w-sm rounded-2xl border border-border bg-card p-6 shadow-xl mt-[15vh]">
            <h2 className="text-lg font-semibold mb-2">Delete zone</h2>
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
                disabled={deleteZone.isPending}
                onClick={async () => {
                  try {
                    await deleteZone.mutateAsync(deleteTarget.id);
                    setDeleteTarget(null);
                    setSubmitError(undefined);
                  } catch (e) {
                    setSubmitError(String(e));
                  }
                }}
              >
                {deleteZone.isPending ? "Deleting…" : "Delete"}
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
              <h2 className="text-lg font-semibold">Edit zone</h2>
              <button
                type="button"
                onClick={() => { setEditTarget(null); setSubmitError(undefined); }}
                className="rounded-lg p-1 hover:bg-muted transition-colors"
              >
                <X className="size-5" />
              </button>
            </div>
            <ZoneForm
              initial={editTarget}
              depots={depots ?? []}
              onSubmit={async (data) => {
                try {
                  await updateZone.mutateAsync({ id: editTarget!.id, data: data as UpdateZoneRequest });
                  setEditTarget(null);
                  setSubmitError(undefined);
                } catch (e) {
                  setSubmitError(String(e));
                }
              }}
              onCancel={() => { setEditTarget(null); setSubmitError(undefined); }}
              error={submitError}
              isPending={updateZone.isPending}
            />
          </div>
        </div>
      )}

      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Zones</h1>
          <p className="text-sm text-muted-foreground">
            Define delivery zones and link them to depots.
          </p>
        </div>
        <Button
          onClick={() => setShowCreate(true)}
          className="gap-2"
          disabled={!depots || depots.length === 0}
        >
          <Plus className="size-4" aria-hidden />
          Add zone
        </Button>
      </div>

      {/* Create Form */}
      {showCreate && (
        <div className="mb-6 rounded-2xl border border-border bg-card p-6">
          <h2 className="mb-4 text-base font-semibold">New zone</h2>
          <ZoneForm
            depots={depots ?? []}
            onSubmit={async (data) => {
              try {
                await createZone.mutateAsync(data as CreateZoneRequest);
                setShowCreate(false);
                setSubmitError(undefined);
              } catch (e) {
                setSubmitError(String(e));
              }
            }}
            onCancel={() => { setShowCreate(false); setSubmitError(undefined); }}
            error={submitError}
            isPending={createZone.isPending}
          />
        </div>
      )}

      {submitError && !showCreate && !editTarget && !deleteTarget && (
        <p className="mb-4 text-sm text-destructive">{submitError}</p>
      )}

      {zones && zones.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border p-12 text-center">
          <p className="font-medium">No zones yet</p>
          <p className="mt-1 text-sm text-muted-foreground">
            Click &ldquo;Add zone&rdquo; to create your first delivery zone.
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto rounded-xl border border-border">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border bg-muted/40">
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Name</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Depot</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Boundary</th>
                <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Status</th>
                <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>
              </tr>
            </thead>
            <tbody>
              {zones?.map((z) => (
                <tr key={z.id} className="border-b border-border/50 hover:bg-muted/30 transition-colors">
                  <td className="px-4 py-3 font-medium">{z.name}</td>
                  <td className="px-4 py-3 text-sm text-muted-foreground">
                    {depotMap.get(z.depotId) ?? z.depotName ?? "—"}
                  </td>
                  <td className="px-4 py-3 text-xs font-mono text-muted-foreground max-w-xs truncate" title={z.boundary}>
                    {z.boundary || "—"}
                  </td>
                  <td className="px-4 py-3">
                    <span
                      className={cn(
                        "inline-flex rounded-full px-2 py-0.5 text-xs font-medium",
                        z.isActive
                          ? "bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-200"
                          : "bg-neutral-100 text-neutral-600 dark:bg-neutral-800 dark:text-neutral-400",
                      )}
                    >
                      {z.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-1">
                      <button
                        type="button"
                        onClick={() => setEditTarget(z)}
                        className="inline-flex items-center gap-1 rounded-lg px-3 py-1.5 text-sm text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
                      >
                        <Pencil className="size-3.5" aria-hidden />
                        Edit
                      </button>
                      <button
                        type="button"
                        onClick={() => setDeleteTarget(z)}
                        className="inline-flex items-center gap-1 rounded-lg px-3 py-1.5 text-sm text-destructive/70 hover:bg-destructive/10 hover:text-destructive transition-colors"
                      >
                        <Trash2 className="size-3.5" aria-hidden />
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  );
}
