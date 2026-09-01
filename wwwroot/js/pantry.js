const container = document.querySelector("[data-state-picker]");
const list = document.getElementById("items-list");
const rowTemplate = document.getElementById("item-row-template");

// Adding items now lives in the global quick-add dialog (nav dock). nav.js
// dispatches "moonatna:item-added" after a successful save; each page inserts
// its own row so it owns how rows render.
document.addEventListener("moonatna:item-added", (e) => {
    const item = e.detail;
    if (!item || item.isAdHoc === true) return; // pantry shows tracked items only

    appendRow(item);
    document.getElementById("empty-state")?.remove();
});

function appendRow(item) {
    const row = rowTemplate.content.firstElementChild.cloneNode(true);
    row.dataset.itemId = item.id;
    row.dataset.state = item.state;
    row.querySelector(".item-name").textContent = item.name;

    const img = row.querySelector("img.item-thumb");
    const iconTile = row.querySelector(".item-thumb-icon");
    if (item.imagePath) {
        img.src = item.imagePath;
        img.alt = item.name;
        img.hidden = false;
        iconTile.hidden = true;
    } else {
        // Quick-add doesn't resolve the category icon yet — generic basket
        // icon (already set on the template) until the page is reloaded.
        iconTile.hidden = false;
    }

    // light the dot for the new item's state on the cloned options
    row.querySelectorAll(".state-opt").forEach((opt) => {
        const active = opt.dataset.state === String(item.state);
        opt.classList.toggle("is-active", active);
        opt.setAttribute("aria-pressed", active ? "true" : "false");
    });

    getUncategorizedGroup().querySelector("ul").appendChild(row);
    row.animate(
        [{ opacity: 0, transform: "translateY(8px) scale(0.98)" }, { opacity: 1, transform: "none" }],
        { duration: 280, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)" });
}

// ---------- long-press action sheet: Edit / Delete (pantry only) ----------
const actionsScrim = document.getElementById("itemActionsScrim");
const actionsSheet = document.getElementById("itemActionsSheet");
const actionsList = document.getElementById("itemActionsList");
const actionsConfirmBox = document.getElementById("itemActionsConfirmBox");
const editBtn = document.getElementById("itemActionEdit");
const deleteBtn = document.getElementById("itemActionDelete");
const cancelDeleteBtn = document.getElementById("itemActionCancelDelete");
const confirmDeleteBtn = document.getElementById("itemActionConfirmDelete");

let pressTimer = null;
let pressRow = null;
let pressStart = null;
const LONG_PRESS_MS = 500;
const MOVE_TOLERANCE = 10;

function openActionsSheet(row) {
    pressRow = row;
    actionsList.hidden = false;
    actionsConfirmBox.hidden = true;
    actionsScrim.classList.add("show");
    actionsSheet.classList.add("show");
}

function closeActionsSheet() {
    actionsScrim.classList.remove("show");
    actionsSheet.classList.remove("show");
    pressRow = null;
}

function cancelPress() {
    window.clearTimeout(pressTimer);
    pressTimer = null;
    pressStart = null;
}

list?.addEventListener("pointerdown", (e) => {
    const row = e.target.closest(".item-row");
    if (!row || e.target.closest(".state-opt")) return; // those already handle taps
    pressStart = { x: e.clientX, y: e.clientY };
    pressTimer = window.setTimeout(() => {
        pressTimer = null;
        if (navigator.vibrate) navigator.vibrate(12);
        openActionsSheet(row);
    }, LONG_PRESS_MS);
});

list?.addEventListener("pointermove", (e) => {
    if (!pressTimer || !pressStart) return;
    const dx = e.clientX - pressStart.x;
    const dy = e.clientY - pressStart.y;
    if (Math.hypot(dx, dy) > MOVE_TOLERANCE) cancelPress();
});

["pointerup", "pointercancel", "pointerleave"].forEach((evt) => {
    list?.addEventListener(evt, cancelPress);
});

list?.addEventListener("contextmenu", (e) => {
    if (e.target.closest(".item-row")) e.preventDefault();
});

actionsScrim?.addEventListener("click", closeActionsSheet);

editBtn?.addEventListener("click", () => {
    if (!pressRow || !window.MoonatnaAdd) return;
    const img = pressRow.querySelector("img.item-thumb");
    window.MoonatnaAdd.openEdit({
        id: Number(pressRow.dataset.itemId),
        name: pressRow.querySelector(".item-name").textContent,
        categoryId: pressRow.dataset.categoryId ? Number(pressRow.dataset.categoryId) : null,
        imagePath: img && !img.hidden ? img.getAttribute("src") : null
    });
    closeActionsSheet();
});

deleteBtn?.addEventListener("click", () => {
    actionsList.hidden = true;
    actionsConfirmBox.hidden = false;
});

cancelDeleteBtn?.addEventListener("click", closeActionsSheet);

confirmDeleteBtn?.addEventListener("click", async () => {
    if (!pressRow) return;
    const row = pressRow;
    const itemId = Number(row.dataset.itemId);
    confirmDeleteBtn.disabled = true;
    try {
        const res = await fetch(actionsSheet.dataset.deleteUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId })
        });
        if (res.ok) {
            const group = row.closest(".category-group");
            row.remove();
            if (group && group.querySelector("ul")?.children.length === 0) group.remove();
            if (!list.querySelector(".item-row")) document.getElementById("empty-state")?.removeAttribute("hidden");
        }
    } finally {
        confirmDeleteBtn.disabled = false;
        closeActionsSheet();
    }
});

// Editing can change an item's category, which moves it between the
// grouped sections — simplest correct way to reflect that is a reload
// rather than re-deriving which <section> it now belongs in client-side.
document.addEventListener("moonatna:item-updated", () => {
    window.location.reload();
});

function getUncategorizedGroup() {
    let group = list.querySelector('[data-category-group=""]');
    if (!group) {
        group = document.createElement("section");
        group.className = "category-group";
        group.dataset.categoryGroup = "";

        const heading = document.createElement("h2");
        heading.textContent = container.dataset.uncategorized;

        group.appendChild(heading);
        group.appendChild(document.createElement("ul"));
        list.appendChild(group);
    }
    return group;
}
