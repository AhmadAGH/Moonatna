/* =====================================================
   STATE DOTS — one tap on an option = one state change

   Replaces status-rail.js. The rail element it used to
   hang state off is gone, so the .item-row itself is the
   scope now: it already carries data-item-id and
   data-state. Optimistic UI — rolls back if the server
   rejects the change.
   ===================================================== */
(function () {
    "use strict";

    var container = document.querySelector("[data-state-picker]");

    function apply(row, state) {
        row.querySelectorAll(".state-opt").forEach(function (opt) {
            var active = opt.dataset.state === String(state);
            opt.classList.toggle("is-active", active);
            opt.setAttribute("aria-pressed", active ? "true" : "false");
        });
        row.dataset.state = String(state);
    }

    function setState(row, state) {
        var prev = row.dataset.state;
        apply(row, state);

        if (!container || !container.dataset.setStateUrl) return;

        fetch(container.dataset.setStateUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId: Number(row.dataset.itemId), state: Number(state) })
        }).then(function (res) {
            if (!res.ok) apply(row, prev);
        }).catch(function () {
            apply(row, prev);
        });
    }

    /* delegation covers rows added later by pantry.js (quick-add) too */
    document.addEventListener("click", function (e) {
        var opt = e.target.closest(".state-opt");
        if (!opt || opt.classList.contains("is-readonly") || opt.classList.contains("is-active")) return;
        var row = opt.closest(".item-row");
        if (row) setState(row, opt.dataset.state);
    });

    /* arrow keys walk the three options — RTL-aware */
    document.addEventListener("keydown", function (e) {
        if (e.key !== "ArrowLeft" && e.key !== "ArrowRight") return;
        var opt = e.target.closest(".state-opt");
        if (!opt) return;
        var row = opt.closest(".item-row");
        if (!row) return;

        var opts = Array.prototype.slice.call(row.querySelectorAll(".state-opt"));
        var i = opts.indexOf(opt);
        var rtl = getComputedStyle(row).direction === "rtl";
        var delta = (e.key === "ArrowRight" ? 1 : -1) * (rtl ? -1 : 1);
        var next = opts[(i + delta + opts.length) % opts.length];
        next.focus();
        next.click();
    });
})();
