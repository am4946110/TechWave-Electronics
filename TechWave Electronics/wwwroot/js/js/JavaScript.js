document.addEventListener("DOMContentLoaded", function () {
    var rows = document.querySelectorAll("#audienceTable tbody tr");
    var rowsPerPage = 10;
    var currentPage = 1;
    var totalPages = Math.ceil(rows.length / rowsPerPage);

    function showPage(page) {
        currentPage = page;
        rows.forEach(function (row) {
            row.style.display = "none";
        });
        var start = (page - 1) * rowsPerPage;
        var end = start + rowsPerPage;
        for (var i = start; i < end && i < rows.length; i++) {
            rows[i].style.display = "";
        }
        updatePagination();
    }

    function updatePagination() {
        var pagination = document.getElementById("pagination");
        pagination.innerHTML = "";

        var prev = document.createElement("li");
        prev.className = "page-item" + (currentPage === 1 ? " disabled" : "");
        var prevLink = document.createElement("a");
        prevLink.className = "page-link";
        prevLink.href = "#";
        prevLink.textContent = "Previous";
        prevLink.onclick = function (e) {
            e.preventDefault();
            if (currentPage > 1) {
                showPage(currentPage - 1);
            }
        };
        prev.appendChild(prevLink);
        pagination.appendChild(prev);

        for (var i = 1; i <= totalPages; i++) {
            var li = document.createElement("li");
            li.className = "page-item" + (i === currentPage ? " active" : "");
            var link = document.createElement("a");
            link.className = "page-link";
            link.href = "#";
            link.textContent = i;
            link.onclick = (function (page) {
                return function (e) {
                    e.preventDefault();
                    showPage(page);
                }
            })(i);
            li.appendChild(link);
            pagination.appendChild(li);
        }

        var next = document.createElement("li");
        next.className = "page-item" + (currentPage === totalPages ? " disabled" : "");
        var nextLink = document.createElement("a");
        nextLink.className = "page-link";
        nextLink.href = "#";
        nextLink.textContent = "Next";
        nextLink.onclick = function (e) {
            e.preventDefault();
            if (currentPage < totalPages) {
                showPage(currentPage + 1);
            }
        };
        next.appendChild(nextLink);
        pagination.appendChild(next);
    }

    showPage(1);

   

});


