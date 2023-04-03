if (document.getElementById("Longitude").value == 0 && document.getElementById("Latitude").value == 0) {
    var map = L.map('map').setView([50.850340, 4.351710], 10);
} else {
    var map = L.map('map').setView([document.getElementById("Latitude").value, document.getElementById("Longitude").value], 10);
}





L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=3zUmUNFLWS2YF5Fsy8iw', {
    tileSize: 512,
    zoomOffset: -1,
    minZoom: 1,
    attribution: "\u003ca href=\"https://www.maptiler.com/copyright/\" target=\"_blank\"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e",
    crossOrigin: true
}).addTo(map);

if (document.getElementById("Longitude").value == 0 && document.getElementById("Latitude").value == 0) {
    var marker = L.marker([50.850340, 4.351710], {
        draggable: true,

    }).addTo(map);
} else {
    var marker = L.marker([document.getElementById("Latitude").value, document.getElementById("Longitude").value], {
        draggable: true,

    }).addTo(map);
}

document.getElementById("Longitude").value = marker.getLatLng().lng;
document.getElementById("Latitude").value = marker.getLatLng().lat;

marker.addEventListener("move", function () {
    document.getElementById("Longitude").value = marker.getLatLng().lng;
    document.getElementById("Latitude").value = marker.getLatLng().lat;


});