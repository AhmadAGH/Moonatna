/* =====================================================
   STATE RAIL — sliding-pill status control

   Replaces state-picker.js (radial). One tap = one state
   change; posts to ItemsController.SetState using the same
   call state-picker.js made (URL from data-set-state-url
   on the [data-state-picker] container, JSON body
   { itemId, state }). Optimistic UI — rolls back if the
   server rejects the change.
   ===================================================== */
(function () {
    "use strict";

    var container = document.querySelector("[data-state-picker]");

    function apply(rail, state) {
        rail.querySelectorAll(".state-seg").forEach(function (seg) {
            var active = seg.dataset.state === String(state);
            seg.classList.toggle("is-active", active);
            seg.setAttribute("aria-checked", active ? "true" : "false");
        });
        rail.dataset.current = String(state);

        var row = rail.closest(".item-row");
        if (row) row.dataset.state = String(state);
    }

    function setState(rail, state) {
        var prev = rail.dataset.current;
        apply(rail, state);

        if (!container || !container.dataset.setStateUrl) return;

        fetch(container.dataset.setStateUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId: Number(rail.dataset.itemId), state: Number(state) })
        }).then(function (res) {
            if (!res.ok) apply(rail, prev);
        }).catch(function () {
            apply(rail, prev);
        });
    }

    /* one-tap state change — delegation covers rows added
       later by pantry.js (quick-add) too */
    document.addEventListener("click", function (e) {
        var seg = e.target.closest(".state-seg");
        if (!seg) return;
        var rail = seg.closest(".state-rail");
        if (!rail || seg.classList.contains("is-active")) return;
        setState(rail, seg.dataset.state);
    });

    /* radiogroup keyboard support — RTL-aware arrows */
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
