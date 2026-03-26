export interface Zone {
  id: string;
  name: string;
  /** WKT polygon boundary, e.g. "POLYGON ((lon lat, ...))" */
  boundary: string;
  isActive: boolean;
  depotId: string;
  depotName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface ZoneBoundaryInput {
  geoJson?: string;
  coordinates?: [number, number][];
  boundaryWkt?: string;
}

export interface CreateZoneRequest {
  name: string;
  depotId: string;
  isActive: boolean;
  boundaryInput: ZoneBoundaryInput;
}

export interface UpdateZoneRequest {
  name: string;
  depotId: string;
  isActive: boolean;
  boundaryInput: ZoneBoundaryInput;
}
