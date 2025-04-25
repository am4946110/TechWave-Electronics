function calculateTotal() {
    let quantity = parseFloat(document.getElementById("Quantity").value) || 0;
    let unitPrice = parseFloat(document.getElementById("UnitPrice").value) || 0;
    let discount = parseFloat(document.getElementById("Discount").value) || 0;
    let tax = parseFloat(document.getElementById("Tax").value) || 0;

    let totalPrice = (quantity * unitPrice) - discount;
    totalPrice = totalPrice < 0 ? 0 : totalPrice; 

    let grandTotal = totalPrice + (totalPrice * (tax / 100));

    document.getElementById("TotalPrice").value = totalPrice.toFixed(2);
    document.getElementById("GrandTotal").value = grandTotal.toFixed(2);
}

document.addEventListener("DOMContentLoaded", function () {
    let inputs = document.querySelectorAll("#Quantity, #UnitPrice, #Discount, #Tax");

    inputs.forEach(input => {
        input.addEventListener("input", calculateTotal);
    });
});