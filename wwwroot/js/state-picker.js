// Shared state picker (Mojoud / Orido / Mukhlis) used by pantry and shopping.
// Pages mark their list container with [data-state-picker] and provide:
// data-set-state-url, data-label-0, data-label-1, data-label-2.
// After a successful POST it dispatches "itemstatechanged" with { itemId, state, badge, row }.
(function () {
    const container = document.querySelector("[data-state-picker]");
    if (!container) return;

    const labels = [container.dataset.label0, container.dataset.label1, container.dataset.label2];
    let picker = null;
    let activeBadge = null;

    function closePicker() {
        picker?.remove();
        picker = null;
        activeBadge = null;
    }

    function openPicker(badge) {
        closePicker();
        activeBadge = badge;
        picker = document.createElement("div");
        picker.className = "state-picker";

        labels.forEach((label, value) => {
            const option = document.createElement("button");
            option.type = "button";
            option.className = `state-option state-${value}`;
            option.textContent = label;
            option.addEventListener("click", () => setState(value));
            picker.appendChild(option);
        });

        document.body.appendChild(picker);
        const rect = badge.getBoundingClientRect();
        picker.style.top = `${rect.bottom + window.scrollY + 6}px`;
        picker.style.left = `${rect.left + window.scrollX + rect.width / 2}px`;
    }

    async function setState(value) {
        const badge = activeBadge;
        closePicker();

        const row = badge.closest("[data-item-id]");
        const itemId = Number(row.dataset.itemId);

        const response = await fetch(container.dataset.setStateUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId, state: value })
        });
        if (!response.ok) return;

        badge.dataset.state = value;
        badge.textContent = labels[value];
        badge.className = `state-badge state-${value}`;

        container.dispatchEvent(new CustomEvent("itemstatechanged", {
            detail: { itemId, state: value, badge, row }
        }));
    }

    container.addEventListener("click", (e) => {
        const badge = e.target.closest(".state-badge");
        if (!badge) return;
        e.stopPropagation();
        openPicker(badge);
    });

    document.addEventListener("click", (e) => {
        if (picker && !picker.contains(e.target)) closePicker();
    });
})();
