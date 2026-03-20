import { describe, it, expect, vi, beforeEach } from "vitest";
import { apiFetch } from "../api";

const mockFetch = vi.fn();
global.fetch = mockFetch;

describe("apiFetch", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should fetch data successfully", async () => {
    const mockData = { id: 1, name: "Test" };

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: async () => mockData,
    });

    const result = await apiFetch("/test");

    expect(result).toEqual(mockData);
    expect(global.fetch).toHaveBeenCalledWith(
      expect.stringContaining("/test"),
      expect.objectContaining({
        headers: expect.objectContaining({
          "Content-Type": "application/json",
        }),
      })
    );
  });

  it("should throw error on failed request", async () => {
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 404,
    });

    await expect(apiFetch("/not-found")).rejects.toThrow("API error: 404");
  });
});
