const container = document.querySelector("[data-state-picker]");
const list = document.getElementById("items-list");
const rowTemplate = document.getElementById("item-row-template");

// data-label-N is read via getAttribute: dataset does not camelCase dash-digit keys.
const stateLabels = [0, 1, 2].map((v) => container.getAttribute(`data-label-${v}`));

// Adding items now lives in the global quick-add dialog (nav dock). When the
// backend wiring lands, nav.js dispatches "moonatna:item-added" after a
// successful POST; each page inserts its own row so it owns how rows render.
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

    const chip = row.querySelector(".state-chip");
    chip.className = `state-chip state-${item.state}`;
    chip.textContent = stateLabels[item.state];

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
