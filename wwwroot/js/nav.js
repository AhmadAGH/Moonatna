// Nav dock interactions — FAB fan, add dialog, avatar menu, toast.
// Submit: POST /Items/Add (JSON), optional photo upload, then broadcast
// "moonatna:item-added" so the current page inserts its own row.
(function () {
    "use strict";

    var dock = document.getElementById("navDock");
    var fab = document.getElementById("navFab");
    var scrim = document.getElementById("navScrim");
    var dialog = document.getElementById("addDialog");
    if (!dock || !fab || !scrim || !dialog) return;

    var titleEl = document.getElementById("addDialogTitle");
    var adhocTag = document.getElementById("adhocTag");
    var catsField = document.getElementById("addCatsField");
    var catsBox = document.getElementById("addCats");
    var photoField = document.getElementById("addPhotoField");
    var photoBtn = document.getElementById("addPhotoBtn");
    var photoInput = document.getElementById("addPhotoInput");
    var photoContent = document.getElementById("addPhotoContent");
    var nameInput = document.getElementById("addNameInput");
    var form = document.getElementById("addItemForm");
    var toast = document.getElementById("addToast");
    var submitBtn = form ? form.querySelector(".add-submit") : null;

    var titles = {
        quick: dialog.dataset.titleQuick || "",
        adhoc: dialog.dataset.titleAdhoc || "",
        full: dialog.dataset.titleFull || ""
    };
    var addedText = dialog.dataset.addedText || "✓";
    var errorText = dialog.dataset.errorText || "✕";

    var toastTimer = null;

    function openDock() {
        dock.classList.add("open");
        scrim.classList.add("show");
        fab.setAttribute("aria-expanded", "true");
    }

    function closeDock() {
        dock.classList.remove("open");
        fab.setAttribute("aria-expanded", "false");
        if (!dialog.classList.contains("show")) scrim.classList.remove("show");
    }

    function setMode(mode) {
        titleEl.textContent = titles[mode] || titles.quick;
        adhocTag.classList.toggle("hidden", mode !== "adhoc");
        photoField.classList.toggle("hidden", mode === "quick");
        var hasCats = catsBox && catsBox.children.length > 0;
        catsField.classList.toggle("hidden", mode !== "full" || !hasCats);
        resetPhoto();
        closeDock();
        scrim.classList.add("show");
        dialog.classList.add("show");
        dialog.dataset.mode = mode;
        window.setTimeout(function () { if (nameInput) nameInput.focus(); }, 300);
    }

    function closeDialog() {
        dialog.classList.remove("show");
        if (!dock.classList.contains("open")) scrim.classList.remove("show");
    }

    function resetPhoto() {
        if (!photoInput || !photoContent) return;
        photoInput.value = "";
        var label = photoContent.dataset.label || "";
        photoContent.innerHTML = "";
        photoContent.insertAdjacentHTML("beforeend",
            '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" width="19" height="19"><path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z"/><circle cx="12" cy="13" r="4"/></svg>');
        var span = document.createElement("span");
        span.textContent = label;
        photoContent.appendChild(span);
    }

    function showToast(text) {
        if (!toast) return;
        toast.textContent = text;
        toast.classList.add("show");
        window.clearTimeout(toastTimer);
        toastTimer = window.setTimeout(function () { toast.classList.remove("show"); }, 1800);
    }

    fab.addEventListener("click", function () {
        if (dock.classList.contains("open")) closeDock(); else openDock();
    });

    document.querySelectorAll(".nav-sat").forEach(function (sat) {
        sat.addEventListener("click", function () { setMode(sat.dataset.mode || "quick"); });
    });

    scrim.addEventListener("click", function () { closeDock(); closeDialog(); });

    var closeBtn = document.getElementById("addDialogClose");
    if (closeBtn) closeBtn.addEventListener("click", closeDialog);

    document.addEventListener("keydown", function (e) {
        if (e.key !== "Escape") return;
        closeDock();
        closeDialog();
        var menu = document.getElementById("avatarMenu");
        if (menu) menu.classList.remove("open");
    });

    // photo: local preview; the upload itself happens after the item is created
    if (photoBtn && photoInput) {
        photoBtn.addEventListener("click", function () { photoInput.click(); });
        photoInput.addEventListener("change", function () {
            var f = photoInput.files && photoInput.files[0];
            if (!f) return;
            photoContent.innerHTML = "";
            var img = document.createElement("img");
            img.className = "add-photo-preview";
            img.src = URL.createObjectURL(f);
            photoContent.appendChild(img);
            var span = document.createElement("span");
            span.textContent = f.name.length > 22 ? f.name.slice(0, 20) + "…" : f.name;
            photoContent.appendChild(span);
        });
    }

    // category chip single-select — tapping the selected chip clears it (optional field)
    if (catsBox) {
        catsBox.addEventListener("click", function (e) {
            var chip = e.target.closest(".add-cat");
            if (!chip) return;
            var wasSelected = chip.classList.contains("sel");
            catsBox.querySelectorAll(".add-cat").forEach(function (c) { c.classList.remove("sel"); });
            if (!wasSelected) chip.classList.add("sel");
        });
    }

    // submit: save the item, then the photo (best-effort), then tell the page
    if (form) {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();
            var name = nameInput.value.trim();
            if (!name) { nameInput.focus(); return; }

            var mode = dialog.dataset.mode || "quick";
            var selChip = catsBox ? catsBox.querySelector(".add-cat.sel") : null;
            var categoryId = mode === "full" && selChip ? Number(selChip.dataset.categoryId) : null;

            if (submitBtn) submitBtn.disabled = true;
            try {
                var res = await fetch(dialog.dataset.addUrl, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ name: name, isAdHoc: mode === "adhoc", categoryId: categoryId })
                });
                if (!res.ok) throw new Error("add failed");
                var item = await res.json();

                var file = photoInput && photoInput.files && photoInput.files[0];
                if (file) {
                    try {
                        var fd = new FormData();
                        fd.append("ItemId", String(item.id));
                        fd.append("Photo", file);
                        var up = await fetch(dialog.dataset.uploadUrl, { method: "POST", body: fd });
                        if (up.ok) {
                            var u = await up.json();
                            if (u && u.imagePath) item.imagePath = u.imagePath;
                        }
                    } catch (uploadErr) { /* photo is best-effort — the item itself is saved */ }
                }

                document.dispatchEvent(new CustomEvent("moonatna:item-added", { detail: item }));
                closeDialog();
                form.reset();
                resetPhoto();
                showToast(addedText);
            } catch (err) {
                showToast(errorText);
            } finally {
                if (submitBtn) submitBtn.disabled = false;
            }
        });
    }

    // avatar menu
    var avatarBtn = document.getElementById("avatarBtn");
    var avatarMenu = document.getElementById("avatarMenu");
    if (avatarBtn && avatarMenu) {
        avatarBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            avatarMenu.classList.toggle("open");
        });
        document.addEventListener("click", function (e) {
            if (!avatarMenu.contains(e.target)) avatarMenu.classList.remove("open");
        });
    }
})();
