const container = document.querySelector("[data-state-picker]");
const list = document.getElementById("items-list");
const copyBtn = document.getElementById("copy-list");
const emptyState = document.getElementById("empty-state");

// نسخ القائمة — item names only, one per line, in display order.
copyBtn.addEventListener("click", async () => {
    const names = [...list.querySelectorAll(".item-name")].map((el) => el.textContent.trim());
    await navigator.clipboard.writeText(names.join("\n"));

    const original = copyBtn.textContent;
    copyBtn.textContent = copyBtn.dataset.copiedText;
    setTimeout(() => { copyBtn.textContent = original; }, 1500);
});

// Purchase — every outcome leaves the list, so the row just goes away.
list.addEventListener("click", async (e) => {
    const purchaseBtn = e.target.closest(".purchase-btn");
    if (!purchaseBtn) return;

    const row = purchaseBtn.closest("[data-item-id]");
    const response = await fetch(container.dataset.purchaseUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ itemId: Number(row.dataset.itemId) })
    });
    if (response.ok) removeRow(row);
});

// Setting an item to Available (0) takes it off the shopping list.
container.addEventListener("itemstatechanged", (e) => {
    if (e.detail.state === 0) removeRow(e.detail.row);
});

function removeRow(row) {
    const group = row.closest("[data-category-group]");
    row.remove();
    if (group && !group.querySelector(".item-row")) group.remove();

    if (!list.querySelector(".item-row")) {
        emptyState.hidden = false;
        copyBtn.hidden = true;
    }
}
