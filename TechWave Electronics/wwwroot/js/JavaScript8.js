document.addEventListener('DOMContentLoaded', function () {
    // التحقق من وجود عنصر imageInput و urlError قبل إضافة المستمع
    const imageInput = document.getElementById('imageInput');
    const errorElement = document.getElementById('urlError');
    if (imageInput && errorElement) {
        imageInput.addEventListener('input', function () {
            const url = this.value;
            const urlPattern = new RegExp(
                '^(https?:\\/\\/)?' + // البروتوكول
                '((([a-zA-Z\\d]([a-zA-Z\\d-]*[a-zA-Z\\d])*)\\.)+[a-zA-Z]{2,}|' + // اسم النطاق
                '((\\d{1,3}\\.){3}\\d{1,3}))' + // أو عنوان IP
                '(\\:\\d+)?(\\/[-a-zA-Z\\d%@_.~+&:]*)*' + // المنفذ والمسار
                '(\\?[;&a-zA-Z\\d%@_.,~+&:=-]*)?' + // سلسلة الاستعلام
                '(\\#[-a-zA-Z\\d_]*)?$', // الجزء المرجعي
                'i'
            );
            if (urlPattern.test(url)) {
                errorElement.style.display = 'none';
            } else {
                errorElement.style.display = 'block';
            }
        });
    }

   
});
document.addEventListener('DOMContentLoaded', function () {
    const countrySelect = document.getElementById('country');
    const citySelect = document.getElementById('city');

    fetch('https://api.countrystatecity.in/v1/countries', {
        headers: {
            'X-CSCAPI-KEY': 'NHhvOEcyWk50N2Vna3VFTE00bFp3MjFKR0ZEOUhkZlg4RTk1MlJlaA=='
        }
    })
        .then(response => response.json())
        .then(data => {
            data.forEach(country => {
                const option = document.createElement('option');
                option.value = country.iso2;
                option.textContent = country.name;
                countrySelect.appendChild(option);
            });
        })
        .catch(error => console.error('خطأ في تحميل قائمة البلدان:', error));

    countrySelect.addEventListener('change', function () {
        const countryCode = this.value;
        citySelect.innerHTML = ''; // مسح الخيارات السابقة

        fetch(`https://api.countrystatecity.in/v1/countries/${countryCode}/cities`, {
            headers: {
                'X-CSCAPI-KEY': 'NHhvOEcyWk50N2Vna3VFTE00bFp3MjFKR0ZEOUhkZlg4RTk1MlJlaA=='
            }
        })
            .then(response => response.json())
            .then(data => {
                data.forEach(city => {
                    const option = document.createElement('option');
                    option.value = city.name;
                    option.textContent = city.name;
                    citySelect.appendChild(option);
                });
            })
            .catch(error => console.error('خطأ في تحميل قائمة المدن:', error));
    });
});
