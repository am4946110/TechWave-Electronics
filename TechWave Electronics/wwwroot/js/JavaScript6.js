function populateDepartmentOptions() {
    const departmentNames = [
        "Customer Service",
        "HR",
        "Storekeeper",
        "Sales",
        "Finance",
        "IT",
        "Management"
    ];

    const selectElement = document.getElementById("departmentSelect");

    if (!selectElement) {
        console.error("There is no select element with ID 'departmentSelect'.");
        return;
    }

    departmentNames.forEach(dept => {
        const option = document.createElement("option");
        option.value = dept;
        option.textContent = dept;
        selectElement.appendChild(option);
    });
}

document.addEventListener("DOMContentLoaded", populateDepartmentOptions);


function populateRoleModel() {
    const departmentNames = [
        "Customer Service",
        "Customer Service Manager",
        "HR",
        "HR Manager",
        "Storekeeper",
        "Warehouse Manager",
        "Sales",
        "Sales Manager",
        "Finance",
        "Finance Manager",
        "It",
        "It Management",
        "Management",
        "Administrators",
        "client"
    ];

    const selectElement = document.getElementById("RoleModelSelect");

    if (!selectElement) {
        console.error("There is no select element with ID 'Role Model Select'.");
        return;
    }

    departmentNames.forEach(dept => {
        const option = document.createElement("option");
        option.value = dept;
        option.textContent = dept;
        selectElement.appendChild(option);
    });
}

document.addEventListener("DOMContentLoaded", populateRoleModel);


function PaymentMethod() {
    const departmentNames = [
        "Cash",
        "Credit Card",
        "PayPal",
        "Bank Transfer",
        "Electronic wallet",
    ];

    const selectElement = document.getElementById("PaymentMethod");

    if (!selectElement) {
        console.error("There is no select element with ID 'PaymentMethod'.");
        return;
    }

    departmentNames.forEach(dept => {
        const option = document.createElement("option");
        option.value = dept;
        option.textContent = dept;
        selectElement.appendChild(option);
    });
}

document.addEventListener("DOMContentLoaded", PaymentMethod);

function PaymentStatus() {
    const departmentNames = [
        "Pending",
        "Paid",
        "Failed",
        "Refunded"
    ];

    const selectElement = document.getElementById("PaymentStatus");

    if (!selectElement) {
        console.error("There is no select element with ID 'PaymentStatus'.");
        return;
    }

    departmentNames.forEach(dept => {
        const option = document.createElement("option");
        option.value = dept;
        option.textContent = dept;
        selectElement.appendChild(option);
    });
}

document.addEventListener("DOMContentLoaded", PaymentStatus);