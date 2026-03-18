import "next-auth";
import "next-auth/jwt";

// ── OpenIddict token endpoint response ────────────────────────────────────────

export interface TokenResponse {
  access_token: string;
  token_type: string;
  expires_in: number;
  refresh_token?: string;
  scope?: string;
}

export interface TokenErrorResponse {
  error: string;
  error_description?: string;
}

// ── Login form ────────────────────────────────────────────────────────────────

export interface LoginFormData {
  email: string;
  password: string;
}

// ── NextAuth type augmentation ────────────────────────────────────────────────

declare module "next-auth" {
  interface Session {
    accessToken: string;
    error?: "RefreshTokenError";
    user: {
      id: string;
      email: string;
      name: string;
      roles: string[];
    };
  }

  interface User {
    accessToken: string;
    refreshToken: string;
    expiresAt: number;
    roles: string[];
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    accessToken: string;
    refreshToken: string;
    expiresAt: number;
    roles: string[];
    error?: "RefreshTokenError";
  }
}
