import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.2/firebase-app.js";
import {
    getAuth,
    signInWithPopup,
    signInWithEmailAndPassword,
    createUserWithEmailAndPassword,
    updateProfile,
    sendPasswordResetEmail,
    GoogleAuthProvider
} from "https://www.gstatic.com/firebasejs/10.12.2/firebase-auth.js";

const root = document.getElementById("auth-root");
const mode = root.dataset.mode;

const app = initializeApp({
    apiKey: root.dataset.apiKey,
    authDomain: root.dataset.authDomain,
    projectId: root.dataset.projectId
});
const auth = getAuth(app);

const form = document.getElementById("auth-form");
const submitBtn = document.getElementById("auth-submit");
const googleBtn = document.getElementById("google-signin");
const errorEl = document.getElementById("auth-error");
const emailInput = document.getElementById("auth-email");
const passwordInput = document.getElementById("auth-password");

function showError(message) {
    errorEl.textContent = message;
    errorEl.hidden = false;
}

function clearError() {
    errorEl.hidden = true;
}

async function completeSignIn(user, forceRefresh = false) {
    const idToken = await user.getIdToken(forceRefresh);
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
}

form.addEventListener("submit", async (event) => {
    event.preventDefault();
    clearError();
    submitBtn.disabled = true;

    try {
        if (mode === "register") {
            const nameInput = document.getElementById("auth-name");
            const confirmInput = document.getElementById("auth-password-confirm");

            if (passwordInput.value !== confirmInput.value) {
                showError(root.dataset.mismatchText);
                submitBtn.disabled = false;
                return;
            }

            const result = await createUserWithEmailAndPassword(auth, emailInput.value, passwordInput.value);
            await updateProfile(result.user, { displayName: nameInput.value });
            // The ID token minted at sign-up predates the profile update, so its
            // "name" claim is still empty — force a refresh to pick it up before
            // posting it to the backend (which reads DisplayName off that claim).
            await completeSignIn(result.user, true);
        } else {
            const result = await signInWithEmailAndPassword(auth, emailInput.value, passwordInput.value);
            await completeSignIn(result.user);
        }
    } catch {
        showError(root.dataset.errorText);
        submitBtn.disabled = false;
    }
});

googleBtn.addEventListener("click", async () => {
    clearError();
    googleBtn.disabled = true;

    try {
        const result = await signInWithPopup(auth, new GoogleAuthProvider());
        await completeSignIn(result.user);
    } catch (err) {
        if (err?.code !== "auth/popup-closed-by-user") {
            showError(root.dataset.errorText);
        }
        googleBtn.disabled = false;
    }
});

const forgotBtn = document.getElementById("forgot-password");
if (forgotBtn) {
    const resetSentEl = document.getElementById("reset-sent");

    forgotBtn.addEventListener("click", async () => {
        clearError();
        resetSentEl.hidden = true;

        if (!emailInput.value) {
            emailInput.focus();
            return;
        }

        try {
            await sendPasswordResetEmail(auth, emailInput.value);
            resetSentEl.hidden = false;
        } catch {
            showError(root.dataset.errorText);
        }
    });
}
