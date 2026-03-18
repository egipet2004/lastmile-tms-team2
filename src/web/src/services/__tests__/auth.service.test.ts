import { describe, it, expect, vi, beforeEach } from "vitest";
import { loginWithPassword, refreshAccessToken } from "../auth.service";

// Mock fetch globally
global.fetch = vi.fn();

describe("auth.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("loginWithPassword", () => {
    it("should return token response on successful login", async () => {
      const mockResponse = {
        access_token: "mock_access_token",
        token_type: "Bearer",
        expires_in: 3600,
        refresh_token: "mock_refresh_token",
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      const result = await loginWithPassword("user@example.com", "password123");

      expect(result).toEqual(mockResponse);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining("/connect/token"),
        expect.objectContaining({
          method: "POST",
          headers: { "Content-Type": "application/x-www-form-urlencoded" },
        })
      );
    });

    it("should throw error on failed login", async () => {
      const mockError = {
        error: "invalid_grant",
        error_description: "Invalid credentials",
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        json: async () => mockError,
      });

      await expect(
        loginWithPassword("user@example.com", "wrong_password")
      ).rejects.toThrow("Invalid credentials");
    });
  });

  describe("refreshAccessToken", () => {
    it("should return new token on successful refresh", async () => {
      const mockResponse = {
        access_token: "new_access_token",
        token_type: "Bearer",
        expires_in: 3600,
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      const result = await refreshAccessToken("mock_refresh_token");

      expect(result).toEqual(mockResponse);
    });

    it("should throw error on failed refresh", async () => {
      const mockError = {
        error: "invalid_grant",
        error_description: "Refresh token expired",
      };

      (global.fetch as any).mockResolvedValueOnce({
        ok: false,
        json: async () => mockError,
      });

      await expect(
        refreshAccessToken("expired_token")
      ).rejects.toThrow("Refresh token expired");
    });
  });
});
