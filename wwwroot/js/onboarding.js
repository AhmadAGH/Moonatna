// Prevent double submits and focus the first input.
document.querySelectorAll(".onboarding form").forEach((form) => {
    form.addEventListener("submit", () => {
        form.querySelector("button[type=submit]").disabled = true;
    });
});

document.querySelector(".onboarding input")?.focus();
