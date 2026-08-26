// MoonatnaLottie — declarative mounts + one-shot bursts.
// lottie-web lazy-loads from CDN on first use; pages degrade gracefully without it.
window.MoonatnaLottie = (function () {
    const PLAYER_URL = "https://cdnjs.cloudflare.com/ajax/libs/lottie-web/5.12.2/lottie.min.js";
    let playerPromise = null;

    function loadPlayer() {
        if (window.lottie) return Promise.resolve();
        if (!playerPromise) {
            playerPromise = new Promise((resolve, reject) => {
                const s = document.createElement("script");
                s.src = PLAYER_URL;
                s.onload = resolve;
                s.onerror = reject;
                document.head.appendChild(s);
            });
        }
        return playerPromise;
    }

    async function mount(el) {
        if (el.offsetParent === null) return; // hidden (e.g. an unrevealed empty state)
        try {
            await loadPlayer();
            const res = await fetch(`/lottie/${el.dataset.lottie}.json`);
            if (!res.ok) return;
            const data = await res.json();
            window.lottie.loadAnimation({
                container: el,
                renderer: "svg",
                loop: true,
                autoplay: true,
                animationData: data
            });
        } catch { /* decoration only — the page works without it */ }
    }

    function mountAll(root = document) {
        root.querySelectorAll("[data-lottie]").forEach(mount);
    }

    async function burstAt(x, y) {
        try {
            await loadPlayer();
            const res = await fetch("/lottie/burst.json");
            if (!res.ok) return;
            const data = await res.json();

            const host = document.createElement("div");
            host.className = "lottie-burst";
            host.style.left = `${x}px`;
            host.style.top = `${y}px`;
            document.body.appendChild(host);

            const anim = window.lottie.loadAnimation({
                container: host,
                renderer: "svg",
                loop: false,
                autoplay: true,
                animationData: data
            });
            anim.addEventListener("complete", () => {
                anim.destroy();
                host.remove();
            });
        } catch { /* decoration only */ }
    }

    document.addEventListener("DOMContentLoaded", () => mountAll());

    return { mountAll, burstAt };
})();
