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
        img.hidden = false;
        iconTile.hidden = true;
    } else {
        // Quick-add doesn't resolve the category icon yet — generic basket
        // icon (already set on the template) until the page is reloaded.
        iconTile.hidden = false;
    }

    // state rail: reflect the new item's state on the cloned rail
    const rail = row.querySelector(".state-rail");
    rail.dataset.itemId = item.id;
    rail.dataset.current = String(item.state);
    rail.setAttribute("aria-label", item.name);
    rail.querySelectorAll(".state-seg").forEach((seg) => {
        const active = seg.dataset.state === String(item.state);
        seg.classList.toggle("is-active", active);
        seg.setAttribute("aria-checked", active ? "true" : "false");
    });

    getUncategorizedGroup().querySelector("ul").appendChild(row);
    row.animate(
        [{ opacity: 0, transform: "translateY(8px) scale(0.98)" }, { opacity: 1, transform: "none" }],
        { duration: 280, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)" });
}

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
