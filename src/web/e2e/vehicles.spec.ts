import { expect, test } from "@playwright/test";

test.describe("Vehicle smoke", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/login");
    await page.getByLabel(/email/i).fill("admin@lastmile.com");
    await page.getByLabel(/password/i).fill("Admin@12345");
    await page.getByRole("button", { name: /^login$/i }).click();
    await expect(page).toHaveURL(/\/dashboard$/, { timeout: 15000 });
  });

  test("vehicles list page renders", async ({ page }) => {
    await page.goto("/vehicles");

    await expect(page.getByRole("heading", { name: /^vehicles$/i })).toBeVisible();
    await expect(page.getByRole("link", { name: /add vehicle/i })).toBeVisible();
    await expect(page.getByRole("columnheader", { name: /plate/i })).toBeVisible();
    await expect(page.getByRole("columnheader", { name: /status/i })).toBeVisible();
  });

  test("vehicle create page renders", async ({ page }) => {
    await page.goto("/vehicles/new");

    await expect(page.getByRole("heading", { name: /add vehicle/i })).toBeVisible();
    await expect(page.getByLabel(/registration plate/i)).toBeVisible();
    await expect(page.getByLabel(/^type$/i)).toBeVisible();
    await expect(page.getByLabel(/parcel capacity/i)).toBeVisible();
    await expect(page.getByLabel(/weight capacity/i)).toBeVisible();
    await expect(page.getByLabel(/^depot$/i)).toBeVisible();
    await expect(page.getByText(/^available$/i).first()).toBeVisible();
    await expect(page.getByRole("button", { name: /create vehicle/i })).toBeVisible();
  });
});

test.describe("Vehicle auth", () => {
  test("unauthenticated user is redirected to login", async ({ page }) => {
    await page.goto("/vehicles");
    await expect(page).toHaveURL(/\/login/, { timeout: 10000 });
  });
});
