// ============================================================================
// Long-press radial state picker — موجود / قرب يخلص / مخلص (pantry only).
//
// Press and hold an item row (~380ms): three options fan out above the finger.
// Slide toward one and release to pick it, or release and tap. Releasing
// without a choice leaves the arc open — it only closes on a selection or on
// the centre close button.
//
// Hit-testing is ANGULAR and computed from the anchor point, not from the
// options' bounding boxes. The old version measured getBoundingClientRect on
// every pointermove, which meant that during the 300ms entrance the targets
// were both tiny (a scaled element reports a scaled rect) and still moving, so
// sliding in the first third of a second highlighted nothing — and once an
// option did light up, its rect grew with the active scale, so the hit radius
// changed under the finger and the highlight flickered. Angles are fixed the
// moment the menu opens, so pointing in a direction is enough; the finger never
// has to land on the button.
//
// NOTE: data-label-0/1/2 must be read via getAttribute — dataset camelCasing
// does not apply to a dash followed by a digit (dataset.label0 is undefined).
// ============================================================================
(function () {
    const container = document.querySelector("[data-state-picker]");
    if (!container) return;

    const labels = [0, 1, 2].map((v) => container.getAttribute(`data-label-${v}`));
    const LONG_PRESS_MS = 380;
    const JITTER_PX = 14;      // thumbs drift; too tight and the press feels broken
    const RADIUS = 112;
    // Was -155 / -90 / -25, which is only 42px of vertical clearance for the two
    // outer options — they sat almost level with the finger and collided with the
    // close button at the centre. Pulling them to -148 / -32 buys 59px.
    const BASE_ANGLES = [-148, -90, -32]; // fan above the press point
    const DEAD_ZONE_PX = 34;   // inside this the finger has not committed
    const ANGLE_TOLERANCE = 46; // degrees either side of an option's bearing
    const UNLOCK_MS = 130;     // grace after the opening release, see unlockSoon()
    const EXIT_MS = 180;
    const EDGE_PAD = 96;       // keeps the fan on screen near the edges

    let pressTimer = null;
    let chargeAnim = null;
    let chargedRow = null;
    let pressStart = null;
    let activeRow = null;
    let picker = null;
    let scrim = null;
    let closeBtn = null;
    let unlockTimer = null;
    let anchor = null;         // { x, y } the arc is drawn around (may be clamped)
    let origin = null;         // { x, y } where the finger actually landed
    let targets = [];          // { value, angle, el } — value null for close
    let currentTarget = null;
    let slideArmed = false;
    let closing = false;

    container.addEventListener("contextmenu", (e) => {
        if (e.target.closest(".item-row")) e.preventDefault();
    });

    container.addEventListener("pointerdown", (e) => {
        if (picker) return;
        const row = e.target.closest(".item-row");
        if (!row) return;

        // Tapping the chip is a shortcut: it opens the arc immediately, anchored
        // on the chip, with sliding disabled since there is no held finger.
        const chip = e.target.closest(".state-chip");
        if (chip) {
            const r = chip.getBoundingClientRect();
            openPicker(row, { x: r.left + r.width / 2, y: r.top + r.height / 2 }, false);
            return;
        }

        activeRow = row;
        pressStart = { x: e.clientX, y: e.clientY };
        pressTimer = setTimeout(() => openPicker(row, pressStart, true), LONG_PRESS_MS);
        chargeAnim = row.animate(
            [{ transform: "scale(1)" }, { transform: "scale(0.975)" }],
            { duration: LONG_PRESS_MS, fill: "forwards", easing: "ease-out" });
    });

    container.addEventListener("pointermove", (e) => {
        if (pressTimer && pressStart &&
            Math.hypot(e.clientX - pressStart.x, e.clientY - pressStart.y) > JITTER_PX) {
            cancelPress();
        }
    });

    document.addEventListener("pointermove", (e) => {
        if (!picker || !slideArmed || closing) return;
        highlight(pick(e.clientX, e.clientY));
    });

    document.addEventListener("pointerup", () => {
        if (pressTimer) {
            cancelPress();
            return;
        }
        if (!picker || closing) return;

        if (slideArmed) {
            const chosen = currentTarget;
            // Releasing without a commitment leaves the arc up so it can be tapped.
            slideArmed = false;
            if (chosen) {
                selectOption(chosen.value);
                return;
            }
        }
        unlockSoon();
    });

    // The arc opens while the finger is still down, so the release that ends the
    // long press also produces a synthesized click at dead centre — right on the
    // close button. A fixed arm delay measured from open does not fix this: it is
    // a race against how long the user happens to hold, and a release just after
    // the delay expires dismisses the menu they only just summoned. Gating on the
    // opening release instead is deterministic. Everything in the picker is
    // click-inert until then, which also stops that phantom click from selecting
    // an option when the arc was opened by tapping the chip.
    function unlockSoon() {
        if (unlockTimer || !picker) return;
        unlockTimer = setTimeout(() => {
            unlockTimer = null;
            picker?.classList.remove("locked");
        }, UNLOCK_MS);
    }

    document.addEventListener("pointercancel", cancelPress);

    function cancelPress() {
        clearTimeout(pressTimer);
        pressTimer = null;
        chargeAnim?.cancel();
        chargeAnim = null;
        pressStart = null;
    }

    // ---- angular hit-testing -------------------------------------------------
    // Inside the dead zone the finger is still on the row it pressed, which counts
    // as "no choice yet" — so sliding back to where you started deselects, and
    // releasing there leaves the arc open.
    function pick(x, y) {
        // The dead zone is measured from where the finger actually landed, but the
        // bearing is measured from the arc's drawn centre. Near a screen edge the
        // two differ, and each has to use the one that is true for it: "has the
        // finger moved yet" is about the finger, "which option is it pointing at"
        // is about what the user can see.
        if (Math.hypot(x - origin.x, y - origin.y) < DEAD_ZONE_PX) return null;

        const dx = x - anchor.x;
        const dy = y - anchor.y;
        if (Math.hypot(dx, dy) < DEAD_ZONE_PX * 0.6) return null;

        const bearing = (Math.atan2(dy, dx) * 180) / Math.PI;
        let best = null;
        let bestDelta = Infinity;
        for (const t of targets) {
            const delta = Math.abs(angleDelta(bearing, t.angle));
            if (delta < bestDelta) { bestDelta = delta; best = t; }
        }
        return bestDelta <= ANGLE_TOLERANCE ? best : null;
    }

    function angleDelta(a, b) {
        return ((((a - b) % 360) + 540) % 360) - 180;
    }

    function highlight(target) {
        if (target === currentTarget) return;
        currentTarget?.el.classList.remove("active");
        target?.el.classList.add("active");
        if (target) navigator.vibrate?.(5);
        currentTarget = target;
    }

    // ---- open / close -------------------------------------------------------
    function openPicker(row, point, slide) {
        cancelPress();
        navigator.vibrate?.(10);
        activeRow = row;
        chargedRow = row;
        slideArmed = slide;
        currentTarget = null;
        closing = false;
        targets = [];

        // Keep the whole fan on screen: clamp horizontally, and flip it below
        // the finger when there is no room above. Rows near the top of the
        // viewport used to lose options off the top edge entirely.
        const flip = point.y < RADIUS + 52;
        origin = { x: point.x, y: point.y };
        anchor = {
            x: Math.min(Math.max(point.x, EDGE_PAD), window.innerWidth - EDGE_PAD),
            y: point.y
        };

        row.animate(
            [{ transform: "scale(1)" }, { transform: "scale(0.975)" }],
            { duration: 140, fill: "forwards", easing: "ease-out" });

        container.classList.add("picker-open");

        scrim = document.createElement("div");
        scrim.className = "radial-scrim";
        scrim.addEventListener("pointerdown", (e) => { e.preventDefault(); closePicker(); });

        picker = document.createElement("div");
        picker.className = "radial-picker locked";
        picker.style.left = `${anchor.x}px`;
        picker.style.top = `${anchor.y}px`;

        labels.forEach((label, value) => {
            const angle = flip ? -BASE_ANGLES[value] : BASE_ANGLES[value];
            const rad = (angle * Math.PI) / 180;

            const opt = document.createElement("button");
            opt.type = "button";
            opt.className = `radial-option state-${value} enter`;
            opt.dataset.value = value;
            opt.textContent = label;
            opt.style.setProperty("--fx", `${Math.cos(rad) * RADIUS}px`);
            opt.style.setProperty("--fy", `${Math.sin(rad) * RADIUS}px`);
            opt.style.setProperty("--d", `${value * 45}ms`);
            opt.addEventListener("click", () => selectOption(value));
            picker.appendChild(opt);
            targets.push({ value, angle, el: opt });
        });

        closeBtn = document.createElement("button");
        closeBtn.type = "button";
        closeBtn.className = "radial-close enter";
        closeBtn.setAttribute("aria-label", container.getAttribute("data-label-close") || "إغلاق");
        closeBtn.innerHTML =
            '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.6" ' +
            'stroke-linecap="round" aria-hidden="true"><path d="M5 5l14 14M19 5L5 19"/></svg>';
        closeBtn.style.setProperty("--fx", "0px");
        closeBtn.style.setProperty("--fy", "0px");
        closeBtn.style.setProperty("--d", "150ms");
        closeBtn.addEventListener("click", closePicker);
        picker.appendChild(closeBtn);

        document.body.append(scrim, picker);

        // Two frames so the entrance styles are guaranteed to have been flushed
        // before they are replaced; one frame is not reliable once the browser
        // batches the append and the class change into a single recalc.
        requestAnimationFrame(() => requestAnimationFrame(() => {
            if (!picker) return;
            scrim.classList.add("show");
            picker.querySelectorAll(".enter").forEach((el) => el.classList.remove("enter"));
        }));

        // A tap-opened arc (the chip shortcut) has no held finger, so its release
        // arrives immediately and unlockSoon() runs from the pointerup handler.
    }

    function closePicker() {
        if (!picker || closing) return;
        closing = true;
        clearTimeout(unlockTimer);
        unlockTimer = null;
        currentTarget?.el.classList.remove("active");

        const dyingPicker = picker;
        const dyingScrim = scrim;
        dyingPicker.classList.add("closing");
        dyingScrim.classList.remove("show");
        setTimeout(() => { dyingPicker.remove(); dyingScrim.remove(); }, EXIT_MS);

        picker = scrim = closeBtn = null;
        currentTarget = null;
        targets = [];
        slideArmed = false;
        anchor = origin = null;
        container.classList.remove("picker-open");

        if (chargedRow) {
            chargedRow.animate(
                [{ transform: "scale(0.975)" }, { transform: "scale(1)" }],
                { duration: 180, easing: "ease-out", fill: "backwards" });
            chargedRow = null;
        }
        activeRow = null;
    }

    async function selectOption(value) {
        const row = activeRow;
        navigator.vibrate?.(12);
        closePicker();
        if (!row) return;

        const itemId = Number(row.dataset.itemId);
        const response = await fetch(container.dataset.setStateUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ itemId, state: value })
        });
        if (!response.ok) return;

        row.dataset.state = value;
        const chip = row.querySelector(".state-chip");
        chip.textContent = labels[value];
        chip.className = `state-chip state-${value}`;
        chip.animate(
            [{ transform: "scale(1.3)" }, { transform: "scale(1)" }],
            { duration: 220, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)" });

        const rect = chip.getBoundingClientRect();
        window.MoonatnaLottie?.burstAt(rect.left + rect.width / 2, rect.top + rect.height / 2);

        container.dispatchEvent(new CustomEvent("itemstatechanged", {
            detail: { itemId, state: value, row }
        }));
    }
})();
