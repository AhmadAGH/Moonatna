// Long-press radial state picker — موجود / ناقص / خلص (pantry only).
// Press and hold an item row (~380ms): options fan out in an arc above the finger.
// Slide onto one and release — or release and tap an option. Tap the backdrop to dismiss.
// NOTE: data-label-0/1/2 must be read via getAttribute — dataset camelCasing
// does not apply to a dash followed by a digit (dataset.label0 would be undefined).
(function () {
    const container = document.querySelector("[data-state-picker]");
    if (!container) return;

    const labels = [0, 1, 2].map((v) => container.getAttribute(`data-label-${v}`));
    const LONG_PRESS_MS = 380;
    const RADIUS = 100;
    const ANGLES = [-155, -90, -25]; // fan above the press point

    let pressTimer = null;
    let chargeAnim = null;
    let chargedRow = null;
    let pressStart = null;
    let activeRow = null;
    let picker = null;
    let scrim = null;
    let currentOption = null;
    let slideArmed = false;

    container.addEventListener("contextmenu", (e) => {
        if (e.target.closest(".item-row")) e.preventDefault();
    });

    container.addEventListener("pointerdown", (e) => {
        if (picker) return;
        const row = e.target.closest(".item-row");
        if (!row) return;

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
            Math.hypot(e.clientX - pressStart.x, e.clientY - pressStart.y) > 10) {
            cancelPress();
        }
    });

    document.addEventListener("pointermove", (e) => {
        if (!picker || !slideArmed) return;
        let found = null;
        picker.querySelectorAll(".radial-option").forEach((opt) => {
            const r = opt.getBoundingClientRect();
            const d = Math.hypot(e.clientX - (r.left + r.width / 2), e.clientY - (r.top + r.height / 2));
            if (d < r.width * 0.8) found = opt;
        });
        if (found !== currentOption) {
            currentOption?.classList.remove("active");
            found?.classList.add("active");
            if (found) navigator.vibrate?.(5);
            currentOption = found;
        }
    });

    document.addEventListener("pointerup", () => {
        if (pressTimer) {
            cancelPress();
            return;
        }
        if (!picker || !slideArmed) return;

        if (currentOption) {
            selectOption(Number(currentOption.dataset.value));
        } else {
            // Released without a selection — keep the arc open so a tap can choose.
            slideArmed = false;
        }
    });

    document.addEventListener("pointercancel", cancelPress);

    function cancelPress() {
        clearTimeout(pressTimer);
        pressTimer = null;
        chargeAnim?.cancel();
        chargeAnim = null;
        pressStart = null;
    }

    function openPicker(row, point, slide) {
        cancelPress();
        navigator.vibrate?.(10);
        activeRow = row;
        chargedRow = row;
        slideArmed = slide;
        currentOption = null;

        row.animate(
            [{ transform: "scale(1)" }, { transform: "scale(0.975)" }],
            { duration: 140, fill: "forwards", easing: "ease-out" });

        scrim = document.createElement("div");
        scrim.className = "radial-scrim";
        scrim.addEventListener("pointerdown", () => closePicker());

        picker = document.createElement("div");
        picker.className = "radial-picker";
        picker.style.left = `${point.x}px`;
        picker.style.top = `${point.y}px`;

        labels.forEach((label, value) => {
            const angle = (ANGLES[value] * Math.PI) / 180;
            const tx = Math.cos(angle) * RADIUS;
            const ty = Math.sin(angle) * RADIUS;

            const opt = document.createElement("button");
            opt.type = "button";
            opt.className = `radial-option state-${value}`;
            opt.dataset.value = value;
            opt.textContent = label;
            opt.addEventListener("click", () => selectOption(value));
            picker.appendChild(opt);

            opt.animate(
                [
                    { transform: "translate(-50%, -50%) translate(0px, 0px) scale(0.2)", opacity: 0 },
                    { transform: `translate(-50%, -50%) translate(${tx}px, ${ty}px) scale(1)`, opacity: 1 }
                ],
                { duration: 300, delay: value * 45, easing: "cubic-bezier(0.34, 1.56, 0.64, 1)", fill: "forwards" });
        });

        document.body.append(scrim, picker);
    }

    function closePicker() {
        picker?.remove();
        scrim?.remove();
        picker = scrim = null;
        currentOption = null;
        slideArmed = false;

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
