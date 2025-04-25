document.getElementById('imageInput').addEventListener('change', function (event) {
    var input = event.target;

    if (input.files && input.files[0]) {
        var file = input.files[0];

        if (!file.type.match('image.*')) {
            alert('Please select a valid image file.');
            return;
        }

        var reader = new FileReader();

        reader.onload = function (e) {
            var imgElement = document.getElementById('imagePreview');
            imgElement.src = e.target.result;
            imgElement.style.display = 'block';
        };

        reader.readAsDataURL(file);
    }
});