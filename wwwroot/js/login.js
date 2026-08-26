import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
import { getAuth, signInWithPopup, GoogleAuthProvider } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";

const root = document.getElementById("login-root");
const button = document.getElementById("google-signin");
const errorEl = document.getElementById("login-error");

const app = initializeApp({
    apiKey: root.dataset.apiKey,
    authDomain: root.dataset.authDomain,
    projectId: root.dataset.projectId
});

const auth = getAuth(app);

button.addEventListener("click", async () => {
    button.disabled = true;
    errorEl.hidden = true;

    try {
        const result = await signInWithPopup(auth, new GoogleAuthProvider());
        const idToken = await result.user.getIdToken();

        const response = await fetch(root.dataset.tokenUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ idToken })
        });

        if (!response.ok) {
            throw new Error("Token exchange failed");
        }

        const data = await response.json();
        window.location.href = data.redirect;
    } catch (err) {
        if (err?.code !== "auth/popup-closed-by-user") {
            errorEl.textContent = root.dataset.errorText;
            errorEl.hidden = false;
        }
        button.disabled = false;
    }
});
