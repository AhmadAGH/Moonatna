const container = document.querySelector("[data-state-picker]");
const addForm = document.getElementById("add-item-form");
const nameInput = document.getElementById("add-item-name");
const list = document.getElementById("items-list");
const rowTemplate = document.getElementById("item-row-template");

// data-label-N is read via getAttribute: dataset does not camelCase dash-digit keys.
const stateLabels = [0, 1, 2].map((v) => container.getAttribute(`data-label-${v}`));

addForm.addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = nameInput.value.trim();
    if (!name) return;

    const response = await fetch(addForm.dataset.addUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, isAdHoc: false, categoryId: null })
    });
    if (!response.ok) return;

    const item = await response.json();
    appendRow(item);
    nameInput.value = "";
    nameInput.focus();
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
