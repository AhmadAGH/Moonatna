// Organize — drag chips between category zones. Long-press (~200ms) to lift on touch,
// plain drag on desktop. Drop on a zone → POST SetCategory. Failure rolls back.
(function () {
    const root = document.querySelector(".organize-page");
    if (!root) return;

    const url = root.dataset.setCategoryUrl;
    const HOLD_MS = 200;

    let pressTimer = null;
    let dragState = null;

    root.addEventListener("pointerdown", (e) => {
        const chip = e.target.closest(".org-chip");
        if (!chip || dragState) return;

        pressTimer = setTimeout(() => startDrag(e, chip), HOLD_MS);
        dragState = { chip, pointerId: e.pointerId, started: false };
    });

    function startDrag(e, chip) {
        dragState.started = true;
        chip.setPointerCapture?.(dragState.pointerId);

        const rect = chip.getBoundingClientRect();
        const ghost = chip.cloneNode(true);
        ghost.classList.add("drag-ghost");
        ghost.style.width = `${rect.width}px`;
        ghost.style.left = `${e.clientX - rect.width / 2}px`;
        ghost.style.top = `${e.clientY - rect.height / 2}px`;
        document.body.appendChild(ghost);

        chip.classList.add("dragging");
        dragState.ghost = ghost;
        dragState.originZone = chip.closest(".zone-items");
        navigator.vibrate?.(8);
    }

    root.addEventListener("pointermove", (e) => {
        if (!dragState || e.pointerId !== dragState.pointerId || !dragState.started) return;

        dragState.ghost.style.left = `${e.clientX - dragState.ghost.offsetWidth / 2}px`;
        dragState.ghost.style.top = `${e.clientY - dragState.ghost.offsetHeight / 2}px`;

        const zone = zoneAt(e.clientX, e.clientY);
        root.querySelectorAll(".category-zone.drop-target")
            .forEach((z) => { if (z !== zone) z.classList.remove("drop-target"); });
        zone?.classList.add("drop-target");
    });

    root.addEventListener("pointerup", (e) => {
        clearTimeout(pressTimer);
        if (!dragState || e.pointerId !== dragState.pointerId) return;
        if (!dragState.started) { dragState = null; return; }

        const { chip, ghost, originZone } = dragState;
        dragState = null;

        const zone = zoneAt(e.clientX, e.clientY);
        root.querySelectorAll(".category-zone.drop-target").forEach((z) => z.classList.remove("drop-target"));
        ghost.remove();
        chip.classList.remove("dragging");

        if (!zone) return;

        const zoneItems = zone.querySelector(".zone-items");
        const categoryId = zoneItems.dataset.zone === "" ? null : Number(zoneItems.dataset.zone);
        if (zoneItems === originZone) return;

        moveChip(chip, zoneItems, categoryId);
    });

    root.addEventListener("pointercancel", () => {
        clearTimeout(pressTimer);
        if (dragState?.started) {
            dragState.ghost.remove();
            dragState.chip.classList.remove("dragging");
        }
        dragState = null;
        root.querySelectorAll(".category-zone.drop-target").forEach((z) => z.classList.remove("drop-target"));
    });

    function zoneAt(x, y) {
        const el = document.elementFromPoint(x, y);
        return el?.closest(".category-zone") ?? null;
    }

    async function moveChip(chip, zoneItems, categoryId) {
        const from = chip.closest(".zone-items");
        zoneItems.appendChild(chip);
        chip.animate(
            [{ transform: "scale(0.6)", opacity: 0 }, { transform: "scale(1)", opacity: 1 }],
            { duration: 220, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)" });
        chip.dataset.categoryId = categoryId ?? "";
        updateCounts();

        const response = await fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId: Number(chip.dataset.itemId), categoryId })
        });

        if (!response.ok) {
            from.appendChild(chip); // roll back
            chip.dataset.categoryId = from.dataset.zone;
            updateCounts();
        }
    }

    function updateCounts() {
        root.querySelectorAll(".category-zone").forEach((zone) => {
            zone.querySelector(".zone-count").textContent = zone.querySelectorAll(".org-chip").length;
        });
    }
})();
