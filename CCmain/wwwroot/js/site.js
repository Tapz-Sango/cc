// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Account dropdown
document.addEventListener("DOMContentLoaded", () => {
    const toggle = document.getElementById("accountToggle");
    const menu = document.getElementById("accountMenu");

    if (!toggle || !menu) return;

    function closeAccountMenu() {
        menu.style.display = "none";
        toggle.setAttribute("aria-expanded", "false");
    }

    toggle.addEventListener("click", (event) => {
        event.stopPropagation();

        const isOpen = menu.style.display !== "none";

        menu.style.display = isOpen ? "none" : "block";
        toggle.setAttribute("aria-expanded", String(!isOpen));
    });

    menu.addEventListener("click", (event) => {
        event.stopPropagation();
    });

    document.addEventListener("click", closeAccountMenu);

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeAccountMenu();
            toggle.focus();
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("accountToggle");
    const menu = document.getElementById("accountMenu");

    if (!toggle || !menu) return;

    function closeMenu() {
        menu.style.display = "none";
        toggle.setAttribute("aria-expanded", "false");
    }

    toggle.addEventListener("click", function (event) {
        event.preventDefault();
        event.stopPropagation();

        const isOpen = menu.style.display === "block";

        menu.style.display = isOpen ? "none" : "block";
        toggle.setAttribute("aria-expanded", String(!isOpen));
    });

    menu.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    document.addEventListener("click", closeMenu);

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeMenu();
        }
    });
});

