document.getElementById("Print").addEventListener("click", function () {
    var content = document.getElementById("printArea").innerHTML;
    var printWindow = window.open('', '', 'height=600,width=800');

    printWindow.document.write('<html><head><title>Manager</title>');
    printWindow.document.write('<style>');
    printWindow.document.write('@media print {');
    printWindow.document.write('  body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #000; background: #fff; }');
    printWindow.document.write('  dl.row { margin: 0; }');
    printWindow.document.write('  dt, dd { padding: 8px 0; border-bottom: 1px solid #ccc; }');
    printWindow.document.write('  dt { font-weight: bold; }');
    printWindow.document.write('  dd.col-sm-10 { text-align: center; }');
    printWindow.document.write('  .btn, nav, header, footer { display: none !important; }');
    printWindow.document.write('  table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }');
    printWindow.document.write('  th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }');
    printWindow.document.write('  .page-break { page-break-after: always; }');
    printWindow.document.write('}');
    printWindow.document.write('</style>');
    printWindow.document.write('</head><body>');
    printWindow.document.write(content);
    printWindow.document.write('</body></html>');

    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
});
