function printDiv(rowId) {
    const row = document.getElementById(rowId);
    const columns = row.getElementsByTagName('td');
    const printWindow = window.open('', '', 'height=400,width=600');

    printWindow.document.write(`
        <html>
        <head>
            <title>Print Reservation</title>
            <style>
                body { text-align: center; font-family: Arial, sans-serif; font-size: 20px; }
                .container { display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100vh; }
                h2 { margin-bottom: 25px; }
                p { font-size: 20px; line-height: 1.8; }
                img { max-width: 100px; margin-bottom: 20px; } 
            </style>
        </head>
        <body>
            <div class="container">
                 <h1>Manager</h1>
                <h2>Booking details</h2>
    `);

    for (let i = 0; i < columns.length; i++) {
        printWindow.document.write(`<p><strong>${columns[i].innerText}</strong></p>`);
    }

    printWindow.document.write(`
            </div>
        </body>
        </html>
    `);

    printWindow.document.close();
    printWindow.print();
}

async function loadBookings() {
    try {
        const response = await fetch('/api/bookings'); // Replace with your API endpoint
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const bookings = await response.json();

        const tableBody = document.getElementById('bookingTableBody');
        bookings.forEach((item, index) => {
            const rowId = `printableRow_${index + 1}`;
            const row = `
                <tr id="${rowId}">
                    <td>${item.id}</td>
                    <td>${item.date}</td>
                    <td>${item.time}</td>
                    <td>${item.service}</td>
                    <td>${item.customer.fullName}</td>
                    <td><button onclick="printDiv('${rowId}')">طباعة</button></td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });
    } catch (error) {
        console.error('Error loading bookings:', error);
    }
}

window.onload = loadBookings;


const bookings = [
    {
        id: 1,
        date: '2025-03-16',
        time: '10:00 AM',
        service: 'Haircut',
        customer: { fullName: 'John Doe' }
    },
    // Add more booking objects as needed
];

window.onload = loadBookings;

function speakReservation(button) {
    const row = button.closest('tr').previousElementSibling;
    const bookingId = row.querySelector('.booking-id').innerText;
    const portNumber = button.closest('tr').querySelector('.port-number').value || "غير محدد";

    const message = `العميل ذو الرقم ${bookingId} عليه التوجه الموظف رقم   ${portNumber}.`;

    if ('speechSynthesis' in window) {
        const speech = new SpeechSynthesisUtterance(message);
        speech.lang = 'ar-EG';  // تعيين اللغة العربية
        speech.rate = 1;        // سرعة الكلام (1 = عادي)
        speech.volume = 1.5;    // أقصى مستوى للصوت
        speech.pitch = 1.5;
        speech.printWindow = 1.5;
        speech.onerror = function (event) {
            console.error("حدث خطأ أثناء تحويل النص إلى كلام:", event.error);
            alert("حدث خطأ أثناء تشغيل الصوت. يرجى المحاولة مرة أخرى.");
        };


        window.speechSynthesis.speak(speech);
    } else {
        alert("المتصفح الخاص بك لا يدعم تحويل النص إلى كلام.");
    }
}


var deviceInfo = {
    userAgent: navigator.userAgent,
    platform: navigator.platform,
    language: navigator.language,
    screenWidth: window.innerWidth || document.documentElement.clientWidth || screen.width,
    screenHeight: window.innerHeight || document.documentElement.clientHeight || screen.height
};

fetch('/Device/SaveDeviceInfo', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(deviceInfo)
})
    .then(response => {
        if (response.ok) {
            console.log('تم إرسال معلومات الجهاز بنجاح.');
        } else {
            console.error('حدث خطأ أثناء إرسال معلومات الجهاز.');
        }
    })
    .catch(error => {
        console.error('خطأ:', error);
    });


