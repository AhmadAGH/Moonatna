// Organize — drag chips between category zones. Long-press (~200ms) to lift on touch,
// plain drag on desktop. Drop on a zone → POST SetCategory. Failure rolls back.
(function () {
    const root = document.querySelector(".organize-page");
    if (!root) return;

    const url = root.dataset.setCategoryUrl;
    const HOLD_MS = 200;

    // auto-scroll while dragging near the viewport edges — the top zone starts
    // below the sticky topbar, the bottom zone above the nav dock
    const EDGE_TOP = 96;
    const EDGE_BOTTOM = 150;
    const MAX_STEP = 16;

    let pressTimer = null;
    let dragState = null;
    let scrollFrame = null;
    let pointerX = 0;
    let pointerY = 0;

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

        pointerX = e.clientX;
        pointerY = e.clientY;
        ensureScrollLoop();
    }

    root.addEventListener("pointermove", (e) => {
        if (!dragState || e.pointerId !== dragState.pointerId || !dragState.started) return;

        pointerX = e.clientX;
        pointerY = e.clientY;

        dragState.ghost.style.left = `${e.clientX - dragState.ghost.offsetWidth / 2}px`;
        dragState.ghost.style.top = `${e.clientY - dragState.ghost.offsetHeight / 2}px`;

        highlightZoneAt(pointerX, pointerY);
    });

    root.addEventListener("pointerup", (e) => {
        clearTimeout(pressTimer);
        if (!dragState || e.pointerId !== dragState.pointerId) return;
        if (!dragState.started) { dragState = null; return; }

        const { chip, ghost, originZone } = dragState;
        dragState = null;
        stopScrollLoop();

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
        stopScrollLoop();
        root.querySelectorAll(".category-zone.drop-target").forEach((z) => z.classList.remove("drop-target"));
    });

    function zoneAt(x, y) {
        const el = document.elementFromPoint(x, y);
        return el?.closest(".category-zone") ?? null;
    }

    function highlightZoneAt(x, y) {
        const zone = zoneAt(x, y);
        root.querySelectorAll(".category-zone.drop-target")
            .forEach((z) => { if (z !== zone) z.classList.remove("drop-target"); });
        zone?.classList.add("drop-target");
    }

    // rAF loop: scrolls while the pointer is parked near an edge mid-drag —
    // no new pointermove events fire when the finger holds still
    function ensureScrollLoop() {
        if (scrollFrame !== null) return;
        scrollFrame = requestAnimationFrame(scrollTick);
    }

    function scrollTick() {
        scrollFrame = null;
        if (!dragState?.started) return;

        const vh = window.innerHeight;
        let step = 0;
        if (pointerY < EDGE_TOP) {
            step = -Math.ceil((1 - pointerY / EDGE_TOP) * MAX_STEP);
        } else if (pointerY > vh - EDGE_BOTTOM) {
            step = Math.ceil((1 - (vh - pointerY) / EDGE_BOTTOM) * MAX_STEP);
        }

        if (step !== 0) {
            window.scrollBy(0, step);
            highlightZoneAt(pointerX, pointerY); // zones shifted under the finger
        }

        scrollFrame = requestAnimationFrame(scrollTick);
    }

    function stopScrollLoop() {
        if (scrollFrame !== null) {
            cancelAnimationFrame(scrollFrame);
            scrollFrame = null;
        }
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
