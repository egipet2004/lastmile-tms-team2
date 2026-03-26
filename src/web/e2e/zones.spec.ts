import { expect, test, type Page } from "@playwright/test";

async function loginAsAdmin(page: Page) {
  await page.goto("/login");
  await page.getByLabel(/email/i).fill("admin@lastmile.com");
  await page.getByLabel(/password/i).fill("Admin@12345");
  await page.getByRole("button", { name: /^login$/i }).click();
  await expect(page).toHaveURL(/\/dashboard$/, { timeout: 15_000 });
}

test.describe("Zones smoke", () => {
  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
  });

  test("zones list page renders", async ({ page }) => {
    await page.goto("/zones");

    await expect(page.getByRole("heading", { name: /^zones$/i })).toBeVisible();
    await expect(page.getByRole("button", { name: /add zone/i })).toBeEnabled({
      timeout: 15_000,
    });

    const emptyState = page.getByText(/no zones yet/i);
    if (await emptyState.isVisible()) {
      await expect(emptyState).toBeVisible();
      return;
    }

    await expect(page.getByRole("columnheader", { name: /zone/i })).toBeVisible();
    await expect(page.getByRole("columnheader", { name: /depot/i })).toBeVisible();
  });

  test("can create a zone", async ({ page }) => {
    const zoneName = `E2E Zone ${Date.now()}`;

    await page.goto("/zones");
    await expect(page.getByRole("button", { name: /add zone/i })).toBeEnabled({
      timeout: 15_000,
    });
    await page.getByRole("button", { name: /add zone/i }).click();

    await expect(page.getByRole("heading", { name: /new zone/i })).toBeVisible();

    await page.getByLabel(/zone name/i).fill(zoneName);
    const depotSelect = page.getByLabel(/^depot$/i);
    await expect
      .poll(
        async () =>
          await depotSelect.evaluate(
            (element) => (element as HTMLSelectElement).options.length,
          ),
        { timeout: 15_000 },
      )
      .toBeGreaterThan(1);

    const selectedDepotLabel = await depotSelect.evaluate((element) => {
      const options = Array.from((element as HTMLSelectElement).options)
        .map((option) => option.text.trim())
        .filter((label, index) => index > 0 && label.length > 0);

      return options[0] ?? "";
    });

    expect(selectedDepotLabel).not.toBe("");
    await depotSelect.selectOption({ label: selectedDepotLabel });
    await page
      .getByPlaceholder(/polygon \(\(lon lat, lon lat, \.\.\.\)\)/i)
      .fill(
        "POLYGON ((145.0 -37.8, 145.1 -37.8, 145.1 -37.7, 145.0 -37.7, 145.0 -37.8))",
      );
    await page.getByRole("button", { name: /create zone/i }).click();

    const createdRow = page.locator("tbody tr", { hasText: zoneName }).first();
    await expect(createdRow).toBeVisible({ timeout: 15_000 });
    await expect(createdRow).toContainText(selectedDepotLabel);
    await expect(createdRow).toContainText("Active");
  });
});
