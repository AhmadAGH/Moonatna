const root = document.querySelector(".recipe-builder");
const rowsHost = document.getElementById("ingredient-rows");
const rowTemplate = document.getElementById("ingredient-row-template");
const addBtn = document.getElementById("add-ingredient");
const saveBtn = document.getElementById("save-recipe");
const nameInput = document.getElementById("recipe-name");
const errorEl = document.getElementById("builder-error");
const prefillEl = document.getElementById("prefill-data");
const removeUrl = root.dataset.removeUrl;
const recipeId = Number(root.dataset.recipeId || 0);

addBtn.addEventListener("click", () => addRow(true));

function addRow(focus, data = null) {
    const row = rowTemplate.content.firstElementChild.cloneNode(true);

    if (data) {
        row.dataset.ingredientId = data.id;
        row.dataset.itemId = data.itemId;
    }

    const nameEl = row.querySelector(".ing-name");
    const qtyEl = row.querySelector(".ing-qty");
    nameEl.placeholder = root.dataset.ingredientPh;
    qtyEl.placeholder = root.dataset.qtyPh;
    nameEl.value = data?.name ?? "";
    qtyEl.value = data?.quantityText ?? "";
    row.querySelector(".ing-optional").checked = data?.isOptional ?? false;
    row.querySelector(".opt-check .opt-label").textContent = root.dataset.optionalLabel;

    const removeBtn = row.querySelector(".remove-row");
    removeBtn.title = root.dataset.removeLabel;
    removeBtn.addEventListener("click", () => onRemove(row));

    rowsHost.appendChild(row);
    row.animate(
        [{ opacity: 0, transform: "translateY(8px)" }, { opacity: 1, transform: "none" }],
        { duration: 220, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)" });
    if (focus) nameEl.focus();
}

async function onRemove(row) {
    const ingredientId = Number(row.dataset.ingredientId || 0);

    if (ingredientId > 0 && removeUrl) {
        try {
            const response = await fetch(removeUrl, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ recipeId, ingredientId })
            });
            if (!response.ok) {
                errorEl.textContent = root.dataset.errorText;
                errorEl.hidden = false;
                return;
            }
        } catch {
            errorEl.textContent = root.dataset.errorText;
            errorEl.hidden = false;
            return;
        }
    }

    errorEl.hidden = true;
    row.animate(
        [{ opacity: 1 }, { opacity: 0, transform: "translateX(12px)" }],
        { duration: 160, easing: "ease-in", fill: "forwards" }
    ).finished.then(() => row.remove()).catch(() => row.remove());
}

saveBtn.addEventListener("click", async () => {
    errorEl.hidden = true;

    const name = nameInput.value.trim();
    const ingredients = [...rowsHost.querySelectorAll(".ingredient-input-row")]
        .map((r, i) => ({
            ingredientId: Number(r.dataset.ingredientId || 0) || null,
            itemId: Number(r.dataset.itemId || 0) || null,
            name: r.querySelector(".ing-name").value.trim(),
            quantityText: r.querySelector(".ing-qty").value.trim() || null,
            isOptional: r.querySelector(".ing-optional").checked,
            isAdHoc: false, // recipe ingredients are staples, created OutOfStock when new
            sortOrder: i    // keep the builder's row order
        }))
        .filter((i) => i.name.length > 0);

    if (!name || ingredients.length === 0) {
        errorEl.textContent = root.dataset.errorText;
        errorEl.hidden = false;
        return;
    }

    saveBtn.disabled = true;
    try {
        const response = await fetch(root.dataset.createUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ id: recipeId, name, photoPath: null, ingredients })
        });
        if (!response.ok) throw new Error("save failed");

        const data = await response.json();
        window.location.href = data.redirect;
    } catch {
        errorEl.textContent = root.dataset.errorText;
        errorEl.hidden = false;
        saveBtn.disabled = false;
    }
});

if (prefillEl) {
    try {
        const rows = JSON.parse(prefillEl.textContent);
        rows.forEach((r) => addRow(false, r));
        if (rows.length === 0) addRow(true);
    } catch {
        addRow(true);
    }
} else {
    addRow(true); // create mode — one row ready on load
}
