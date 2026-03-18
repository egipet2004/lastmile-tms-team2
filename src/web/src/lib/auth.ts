import NextAuth from "next-auth";
import Credentials from "next-auth/providers/credentials";
import { loginWithPassword, refreshAccessToken } from "@/services/auth.service";
import type { JWT } from "next-auth/jwt";

function decodeJwtPayload(token: string): Record<string, unknown> {
  const base64Url = token.split(".")[1];
  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  const json = Buffer.from(base64, "base64").toString("utf-8");
  return JSON.parse(json);
}

export const { handlers, signIn, signOut, auth } = NextAuth({
  pages: {
    signIn: "/login",
  },
  session: {
    strategy: "jwt",
  },
  providers: [
    Credentials({
      name: "OpenIddict",
      credentials: {
        email: { label: "Email", type: "email" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials) {
        const email = credentials.email as string;
        const password = credentials.password as string;

        if (!email || !password) return null;

        try {
          const tokenResponse = await loginWithPassword(email, password);
          const payload = decodeJwtPayload(tokenResponse.access_token);

          // Extract roles — can be string or string[]
          const rawRoles = payload["role"] ?? payload["roles"] ?? [];
          const roles = Array.isArray(rawRoles)
            ? (rawRoles as string[])
            : [rawRoles as string];

          return {
            id: payload["sub"] as string,
            email: (payload["email"] as string) ?? email,
            name: (payload["name"] as string) ?? email,
            accessToken: tokenResponse.access_token,
            refreshToken: tokenResponse.refresh_token ?? "",
            expiresAt: Math.floor(Date.now() / 1000) + tokenResponse.expires_in,
            roles,
          };
        } catch (error) {
          console.error("[auth] Login failed:", error);
          return null;
        }
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user }) {
      // Initial sign in — copy user fields to token
      if (user) {
        token.accessToken = user.accessToken;
        token.refreshToken = user.refreshToken;
        token.expiresAt = user.expiresAt;
        token.roles = user.roles;
        return token;
      }

      // Return existing token if not expired (with 60-second buffer)
      if (Date.now() / 1000 < token.expiresAt - 60) {
        return token;
      }

      // Token expired — try to refresh
      return await refreshToken(token);
    },
    async session({ session, token }) {
      session.accessToken = token.accessToken;
      session.error = token.error;
      session.user.id = token.sub ?? "";
      session.user.roles = token.roles;
      return session;
    },
  },
});

async function refreshToken(token: JWT): Promise<JWT> {
  try {
    const tokenResponse = await refreshAccessToken(token.refreshToken);

    return {
      ...token,
      accessToken: tokenResponse.access_token,
      refreshToken: tokenResponse.refresh_token ?? token.refreshToken,
      expiresAt: Math.floor(Date.now() / 1000) + tokenResponse.expires_in,
      error: undefined,
    };
  } catch (error) {
    console.error("[auth] Token refresh failed:", error);
    return {
      ...token,
      error: "RefreshTokenError",
    };
  }
}
