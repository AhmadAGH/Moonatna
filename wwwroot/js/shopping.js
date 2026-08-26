const section = document.querySelector(".items-page");
const list = document.getElementById("items-list");
const copyBtn = document.getElementById("copy-list");
const emptyState = document.getElementById("empty-state");

const SWIPE_TRIGGER = 88;
let drag = null;

// نسخ القائمة — item names only, one per line, in display order.
copyBtn.addEventListener("click", async () => {
    const names = [...list.querySelectorAll(".item-name")].map((el) => el.textContent.trim());
    await navigator.clipboard.writeText(names.join("\n"));

    const original = copyBtn.textContent;
    copyBtn.textContent = copyBtn.dataset.copiedText;
    setTimeout(() => { copyBtn.textContent = original; }, 1500);
});

// ✓ button — same action as the swipe.
list.addEventListener("click", (e) => {
    const purchaseBtn = e.target.closest(".purchase-btn");
    if (!purchaseBtn) return;
    purchaseRow(purchaseBtn.closest(".swipe-wrap"));
});

// ---- swipe to restock (موجود) — drag leftward, release past the threshold ----
list.addEventListener("pointerdown", (e) => {
    if (e.target.closest(".purchase-btn")) return;
    const row = e.target.closest(".item-row");
    if (!row) return;
    drag = { row, startX: e.clientX, dx: 0, active: false, pointerId: e.pointerId };
});

list.addEventListener("pointermove", (e) => {
    if (!drag || e.pointerId !== drag.pointerId) return;
    const dx = e.clientX - drag.startX;

    if (!drag.active) {
        if (Math.abs(dx) < 8) return;
        drag.active = true;
        drag.row.setPointerCapture?.(drag.pointerId);
    }

    drag.dx = Math.min(0, dx); // leftward only — toward the action side in RTL
    drag.row.style.transform = `translateX(${drag.dx}px)`;
    under(drag.row).style.opacity = Math.min(1, -drag.dx / SWIPE_TRIGGER);
});

list.addEventListener("pointerup", finishSwipe);
list.addEventListener("pointercancel", cancelSwipe);

function finishSwipe() {
    if (!drag) return;
    const { row, dx, active } = drag;
    drag = null;
    if (!active) return;

    if (-dx >= SWIPE_TRIGGER) {
        const wrap = row.closest(".swipe-wrap");
        row.animate(
            [{ transform: `translateX(${dx}px)` }, { transform: "translateX(-115%)" }],
            { duration: 180, easing: "cubic-bezier(0.4, 0, 1, 1)", fill: "forwards" }
        ).finished.then(() => purchaseRow(wrap)).catch(() => { });
    } else {
        snapBack(row);
    }
}

function cancelSwipe() {
    if (!drag) return;
    const { row, active } = drag;
    drag = null;
    if (active) snapBack(row);
}

function snapBack(row) {
    row.style.transition = "transform 0.22s cubic-bezier(0.34, 1.56, 0.64, 1)";
    row.style.transform = "translateX(0)";
    row.addEventListener("transitionend", () => { row.style.transition = ""; }, { once: true });
    under(row).style.opacity = 0;
}

function under(row) {
    return row.closest(".swipe-wrap").querySelector(".swipe-under");
}

// Purchase routes through the 4-outcome logic server-side: staples become
// Available, ad-hoc items promote or archive. All outcomes leave the list.
async function purchaseRow(wrap) {
    const row = wrap.querySelector(".item-row");
    const rect = wrap.getBoundingClientRect();

    const response = await fetch(section.dataset.purchaseUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ itemId: Number(row.dataset.itemId) })
    });
    if (!response.ok) {
        row.getAnimations().forEach((a) => a.cancel());
        row.style.transform = "";
        under(row).style.opacity = 0;
        return;
    }

    window.MoonatnaLottie?.burstAt(rect.left + rect.width / 2, rect.top + rect.height / 2);
    removeRow(wrap);
}

async function removeRow(wrap) {
    const group = wrap.closest("[data-category-group]");
    const height = wrap.offsetHeight;
    try {
        await wrap.animate(
            [{ height: `${height}px`, opacity: 1 }, { height: "0px", opacity: 0 }],
            { duration: 240, easing: "cubic-bezier(0.4, 0, 1, 1)", fill: "forwards" }).finished;
    } catch { }

    wrap.remove();
    if (group && !group.querySelector(".swipe-wrap")) group.remove();

    if (!list.querySelector(".swipe-wrap")) {
        emptyState.hidden = false;
        copyBtn.hidden = true;
        window.MoonatnaLottie?.mountAll(emptyState);
    }
}
