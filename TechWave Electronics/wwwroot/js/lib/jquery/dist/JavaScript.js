$(document).ready(function () {
    /**
     * 
     * @returns {Object} 
     */
    const getDeviceInfo = () => ({
        userAgent: navigator.userAgent,
        platform: navigator.platform,
        language: navigator.language,
        screenWidth: screen.width,
        screenHeight: screen.height
    });

    /**
     * 
     * @param {string} url
     * @param {Object} data 
     */
    function sendDataToServer(url, data) {
        $.post(url, data)
            .done(function (response) {
                console.log('Data sent successfully:', response);
            })
            .fail(function (error) {
                console.error('Error sending data:', error);
            });
    }


    sendDataToServer('/Device/SaveDeviceInfo', getDeviceInfo());
});
