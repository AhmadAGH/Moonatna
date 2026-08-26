const trigger = document.getElementById("menu-trigger");
const pop = document.getElementById("menu-pop");
const modal = document.getElementById("delete-modal");
const deleteOpen = document.getElementById("delete-open");
const deleteCancel = document.getElementById("delete-cancel");

trigger.addEventListener("click", (e) => {
    e.stopPropagation();
    pop.hidden = !pop.hidden;
});

document.addEventListener("click", (e) => {
    if (!pop.hidden && !pop.contains(e.target)) pop.hidden = true;
});

deleteOpen.addEventListener("click", () => {
    pop.hidden = true;
    modal.hidden = false;
});

function closeModal() {
    modal.hidden = true;
}

deleteCancel.addEventListener("click", closeModal);
modal.addEventListener("click", (e) => { if (e.target === modal) closeModal(); });
document.addEventListener("keydown", (e) => { if (e.key === "Escape" && !modal.hidden) closeModal(); });
