import { describe, it, expect } from "vitest";

import { parseApiErrorMessage } from "../network/api";

describe("parseApiErrorMessage", () => {
  it("reads RFC 7807 detail when errors is empty", async () => {
    const res = {
      status: 404,
      text: async () =>
        JSON.stringify({
          title: "Not Found",
          detail: "Vehicle abc was not found.",
        }),
    } as Response;

    await expect(parseApiErrorMessage(res)).resolves.toBe(
      "Vehicle abc was not found.",
    );
  });
});
