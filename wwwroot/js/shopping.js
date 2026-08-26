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

// Purchase — every outcome leaves the list: burst, then collapse the row away.
list.addEventListener("click", async (e) => {
    const purchaseBtn = e.target.closest(".purchase-btn");
    if (!purchaseBtn) return;

    const row = purchaseBtn.closest("[data-item-id]");
    const rect = row.getBoundingClientRect();

    const response = await fetch(container.dataset.purchaseUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ itemId: Number(row.dataset.itemId) })
    });
    if (!response.ok) return;

    window.MoonatnaLottie?.burstAt(rect.left + rect.width / 2, rect.top + rect.height / 2);
    removeRow(row);
});

// Setting an item to موجود (0) takes it off the shopping list.
container.addEventListener("itemstatechanged", (e) => {
    if (e.detail.state !== 0) return;
    const rect = e.detail.row.getBoundingClientRect();
    window.MoonatnaLottie?.burstAt(rect.left + rect.width / 2, rect.top + rect.height / 2);
    removeRow(e.detail.row);
});

async function removeRow(row) {
    const group = row.closest("[data-category-group]");
    const height = row.offsetHeight;
    try {
        await row.animate(
            [{ height: `${height}px`, opacity: 1 }, { height: "0px", opacity: 0 }],
            { duration: 240, easing: "cubic-bezier(0.4, 0, 1, 1)", fill: "forwards" }).finished;
    } catch { }

    row.remove();
    if (group && !group.querySelector(".item-row")) group.remove();

    if (!list.querySelector(".item-row")) {
        emptyState.hidden = false;
        copyBtn.hidden = true;
        window.MoonatnaLottie?.mountAll(emptyState);
    }
}
