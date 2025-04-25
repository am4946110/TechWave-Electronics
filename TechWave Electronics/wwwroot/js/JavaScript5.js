function printRow(button) {
    var row = button.closest("tr").previousElementSibling;
    var printWindow = window.open('', '', 'width=600,height=400');
    printWindow.document.write('<html><head><title>Print Invoice</title>');
    printWindow.document.write('<style>table { width: 100%; border-collapse: collapse; }');
    printWindow.document.write('td, th { border: 1px solid black; padding: 10px; text-align: center; }</style>');
    printWindow.document.write('</head><body>');
    printWindow.document.write('<table>');
    printWindow.document.write('<thead>');
    printWindow.document.write('<tr>');
    printWindow.document.write('<th>Invoice ID</th>');
    printWindow.document.write('<th>Product Name</th>');
    printWindow.document.write('<th>Grand Total</th>');
    printWindow.document.write('<th>Total Price</th>');
    printWindow.document.write('<th>Quantity</th>');
    printWindow.document.write('<th>Customer Name</th>');
    printWindow.document.write('</tr>');
    printWindow.document.write('</thead>');
    printWindow.document.write('<tbody>');
    printWindow.document.write(row.outerHTML);
    printWindow.document.write('</tbody>');
    printWindow.document.write('</table>');
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}

