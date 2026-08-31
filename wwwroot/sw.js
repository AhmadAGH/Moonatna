// Moonatna service worker — caches STATIC assets only. Pages always hit the
// network: pantry/shopping data must never be served stale.
// Bump CACHE when you want clients to drop previously cached assets.
const CACHE = "moonatna-v2";
const CORE = [
    "/css/layout.css",
    "/lottie/burst.json",
    "/lottie/float.json",
    "/img/logo.png"
];

const isStaticAsset = (url) =>
    url.pathname.startsWith("/css/") ||
    url.pathname.startsWith("/js/") ||
    url.pathname.startsWith("/lottie/") ||
    url.pathname.startsWith("/img/");

self.addEventListener("install", (e) => {
    e.waitUntil(caches.open(CACHE).then((c) => c.addAll(CORE)));
    self.skipWaiting();
});

self.addEventListener("activate", (e) => {
    e.waitUntil(
        caches.keys().then((keys) =>
            Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
    );
    self.clients.claim();
});

self.addEventListener("fetch", (e) => {
    if (e.request.method !== "GET") return;

    const url = new URL(e.request.url);

    // Cache-first for own static assets + versioned CDN files (fonts, lottie-web, FA).
    // asp-append-version changes URLs when files change, so updates flow naturally.
    const cdn = ["cdnjs.cloudflare.com", "fonts.googleapis.com", "fonts.gstatic.com"];
    if ((url.origin === self.location.origin && isStaticAsset(url)) || cdn.includes(url.hostname)) {
        e.respondWith(
            caches.match(e.request).then((cached) =>
                cached || fetch(e.request).then((res) => {
                    if (res.ok) {
                        const clone = res.clone();
                        caches.open(CACHE).then((c) => c.put(e.request, clone));
                    }
                    return res;
                }))
        );
    }
    // Everything else (pages, posts) goes straight to the network.
});
