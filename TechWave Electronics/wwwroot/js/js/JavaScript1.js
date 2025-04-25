document.addEventListener("DOMContentLoaded", () => {
    const dropdownItems = document.querySelectorAll(".nav-item.dropdown");

    dropdownItems.forEach((dropdown) => {
        const toggleElement = dropdown.querySelector(".dropdown-toggle");
        const menuElement = dropdown.querySelector(".dropdown-menu");

        if (!toggleElement || !menuElement) return;


        let bsDropdown = new bootstrap.Dropdown(toggleElement);
        
        let hideTimeout = null;

        const delayDuration = 100; 

        dropdown.addEventListener("mouseenter", () => {
            if (hideTimeout) {
                clearTimeout(hideTimeout);
                hideTimeout = null;
            }
            bsDropdown.show();
        });

        dropdown.addEventListener("mouseleave", () => {
            hideTimeout = setTimeout(() => {
                bsDropdown = bootstrap.Dropdown.getInstance(toggleElement);
                if (bsDropdown) {
                    bsDropdown.hide();
                }
            }, delayDuration);
        });
    });
});
