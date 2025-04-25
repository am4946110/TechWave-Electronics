document.addEventListener("DOMContentLoaded", function () {

    setupPagination();
});

function setupPagination() {
    var rows = [...document.querySelectorAll("#orderDetailsTable tbody tr")];
    var rowsPerPage = 10;
    var currentPage = 1;
    var totalPages = Math.ceil(rows.length / rowsPerPage);

    function showPage(page) {
        currentPage = page;
        rows.forEach(row => row.style.display = "none");
        rows.slice((page - 1) * rowsPerPage, page * rowsPerPage)
            .forEach(row => row.style.display = "");
        updatePagination();
    }

    function updatePagination() {
        var pagination = document.getElementById("pagination");
        pagination.innerHTML = "";

        function createPageItem(text, page, isDisabled, isActive) {
            var li = document.createElement("li");
            li.className = `page-item${isDisabled ? " disabled" : ""}${isActive ? " active" : ""}`;
            var link = document.createElement("a");
            link.className = "page-link";
            link.href = "#";
            link.textContent = text;
            link.onclick = function (e) {
                e.preventDefault();
                if (!isDisabled) showPage(page);
            };
            li.appendChild(link);
            pagination.appendChild(li);
        }

        createPageItem("Previous", currentPage - 1, currentPage === 1, false);
        for (let i = 1; i <= totalPages; i++) {
            createPageItem(i, i, false, i === currentPage);
        }
        createPageItem("Next", currentPage + 1, currentPage === totalPages, false);
    }

    showPage(1);
}

document.addEventListener("DOMContentLoaded", function () {
    let labels = [];
    let quantities = [];
    let prices = [];

    document.querySelectorAll("#orderDetailsTable tbody tr").forEach(row => {
        let orderId = row.querySelector("td:nth-child(3)").textContent.trim(); // Order ID
        let quantity = parseInt(row.querySelector("td:nth-child(1)").textContent.trim()) || 0; // Quantity
        let price = parseFloat(row.querySelector(".order-price").textContent.trim()) || 0; // Price

        labels.push(orderId);
        quantities.push(quantity);
        prices.push(price);
    });

    let ctx = document.getElementById("orderChart").getContext("2d");
    new Chart(ctx, {
        type: "bar",
        data: {
            labels: labels,
            datasets: [
                {
                    label: "Quantity",
                    data: quantities,
                    backgroundColor: "rgba(54, 162, 235, 0.5)",
                    borderColor: "rgba(54, 162, 235, 1)",
                    borderWidth: 1
                },
                {
                    label: "Price",
                    data: prices,
                    backgroundColor: "rgba(255, 99, 132, 0.5)",
                    borderColor: "rgba(255, 99, 132, 1)",
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
});
