// Full-width vs grid layout toggle for item cards (pantry + shopping).
// Persisted per page (data-layout-key) so the choice sticks on return visits.
(() => {
    const page = document.querySelector(".items-page[data-layout-key]");
    if (!page) return;

    const storageKey = `moonatna:layout:${page.dataset.layoutKey}`;
    const toggle = page.querySelector(".layout-toggle");
    if (!toggle) return;

    const buttons = [...toggle.querySelectorAll("button[data-layout]")];

    function apply(layout) {
        page.dataset.layout = layout;
        buttons.forEach((btn) => {
            const active = btn.dataset.layout === layout;
            btn.classList.toggle("is-active", active);
            btn.setAttribute("aria-pressed", active ? "true" : "false");
        });
    }

    let saved;
    try { saved = localStorage.getItem(storageKey); } catch { saved = null; }
    apply(saved === "grid" ? "grid" : "full");

    toggle.addEventListener("click", (e) => {
        const btn = e.target.closest("button[data-layout]");
        if (!btn) return;
        apply(btn.dataset.layout);
        try { localStorage.setItem(storageKey, btn.dataset.layout); } catch { /* ignore */ }
    });
})();
