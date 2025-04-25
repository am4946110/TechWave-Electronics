window.addEventListener('load', function () {
    var splashScreen = document.getElementById('splash-screen');
    var content = document.getElementById('content');

    if (splashScreen && content) {
        setTimeout(function () {
            splashScreen.style.display = 'none';
            content.style.display = 'block';
        }, 10000);
    } else {
        console.error('Required elements are not present in the DOM');
    }
});
