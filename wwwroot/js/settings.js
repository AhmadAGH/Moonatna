// Copy join code to clipboard with localized feedback.
const copyBtn = document.getElementById("copy-code");
copyBtn?.addEventListener("click", async () => {
    const code = document.getElementById("join-code").textContent.trim();
    await navigator.clipboard.writeText(code);
    const original = copyBtn.textContent;
    copyBtn.textContent = copyBtn.dataset.copiedText;
    setTimeout(() => { copyBtn.textContent = original; }, 1500);
});

// The switch submits immediately; the save button is only a no-JS fallback.
const form = document.getElementById("autopromote-form");
if (form) {
    document.getElementById("autopromote-save").hidden = true;
    form.querySelector("input[type=checkbox]").addEventListener("change", () => form.submit());
}
