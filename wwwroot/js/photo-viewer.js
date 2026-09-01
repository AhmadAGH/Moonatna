/* =====================================================
   PHOTO VIEWER — opens an item's photo full size.

   Shared by pantry and shopping, so it listens on the
   document and never assumes either page's markup beyond
   img.item-thumb inside an .item-row.

   Both item pages already claim the press on a row:
   shopping drags it to purchase, pantry holds it for the
   edit/delete sheet. So only a real tap counts — one that
   neither moved far nor was held — otherwise a snapped-back
   swipe or a long-press would open the photo as well.
   ===================================================== */
(function () {
    "use strict";

    var viewer = document.getElementById("photoViewer");
    if (!viewer) return;

    var img = document.getElementById("photoViewerImg");
    var caption = document.getElementById("photoViewerCaption");
    var closeBtn = document.getElementById("photoViewerClose");

    var MOVE_TOLERANCE = 10;   // px — beyond this it was a swipe
    var HOLD_MS = 450;         // beyond this the long-press sheet owns it

    var press = null;
    var lastFocus = null;

    function labelFor(thumb) {
        var row = thumb.closest(".item-row");
        var nameEl = row && row.querySelector(".item-name-text, .item-name");
        return nameEl ? nameEl.textContent.trim() : "";
    }

    function open(thumb) {
        var label = labelFor(thumb);
        lastFocus = document.activeElement;

        img.src = thumb.currentSrc || thumb.src;
        img.alt = label;
        caption.textContent = label;

        viewer.classList.add("show");
        document.body.classList.add("photo-open");
        closeBtn.focus();
    }

    function close() {
        viewer.classList.remove("show");
        document.body.classList.remove("photo-open");
        img.removeAttribute("src");
        if (lastFocus && lastFocus.focus) lastFocus.focus();
        lastFocus = null;
    }

    document.addEventListener("pointerdown", function (e) {
        var thumb = e.target.closest("img.item-thumb");
        press = thumb ? { thumb: thumb, x: e.clientX, y: e.clientY, at: Date.now() } : null;
    });

    document.addEventListener("click", function (e) {
        var thumb = e.target.closest("img.item-thumb");
        if (!thumb) return;

        var p = press;
        press = null;
        if (!p || p.thumb !== thumb) return;
        if (Math.hypot(e.clientX - p.x, e.clientY - p.y) > MOVE_TOLERANCE) return;
        if (Date.now() - p.at >= HOLD_MS) return;

        open(thumb);
    });

    /* the thumbs carry role="button" tabindex="0" */
    document.addEventListener("keydown", function (e) {
        if (e.key !== "Enter" && e.key !== " ") return;
        var thumb = e.target.closest && e.target.closest("img.item-thumb");
        if (!thumb) return;
        e.preventDefault();
        open(thumb);
    });

    viewer.addEventListener("click", function (e) {
        // the backdrop closes; the photo itself does not
        if (e.target === viewer || e.target.closest(".photo-viewer-close")) close();
    });

    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape" && viewer.classList.contains("show")) close();
    });
})();
