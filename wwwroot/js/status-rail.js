/* =====================================================
   STATE RAIL — sliding-pill status control

   Replaces state-picker.js (radial) and the long-press
   flow on .state-chip. One tap = one state change;
   the filled pill slides to the tapped segment.
   ===================================================== */
(function () {
    "use strict";

    function setState(rail, state) {
        rail.querySelectorAll(".state-seg").forEach(function (seg) {
            var active = seg.dataset.state === state;
            seg.classList.toggle("is-active", active);
            seg.setAttribute("aria-checked", active ? "true" : "false");
        });
        rail.dataset.current = state;

        /* TODO (server wiring): reuse the exact fetch call
           state-picker.js makes to ItemsController.SetState
           (same URL + antiforgery header), with:
             { id: Number(rail.dataset.itemId), state: Number(state) }
           Optimistic UI is fine — the change is cheap to
           reverse, so update first, roll back on failure. */
    }

    /* One-tap state change (event delegation — works for
       rows added later by pantry.js / quick-add too) */
    document.addEventListener("click", function (e) {
        var seg = e.target.closest(".state-seg");
        if (!seg) return;
        var rail = seg.closest(".state-rail");
        if (!rail || seg.classList.contains("is-active")) return;
        setState(rail, seg.dataset.state);
    });

    /* Radiogroup keyboard support — RTL-aware arrows */
    document.addEventListener("keydown", function (e) {
        if (e.key !== "ArrowLeft" && e.key !== "ArrowRight") return;
        var seg = e.target.closest(".state-seg");
        if (!seg) return;
        var rail = seg.closest(".state-rail");
        var segs = Array.prototype.slice.call(rail.querySelectorAll(".state-seg"));
        var i = segs.indexOf(seg);
        var rtl = getComputedStyle(rail).direction === "rtl";
        var delta = (e.key === "ArrowRight" ? 1 : -1) * (rtl ? -1 : 1);
        var next = segs[(i + delta + segs.length) % segs.length];
        next.focus();
        next.click();
    });
})();
