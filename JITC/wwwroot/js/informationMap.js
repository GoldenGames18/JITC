var map = L.map('map').setView([document.getElementById("Latitude").value, document.getElementById("Longitude").value], 10);
L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=3zUmUNFLWS2YF5Fsy8iw', {
    tileSize: 512,
    zoomOffset: -1,
    minZoom: 1,
    attribution: "\u003ca href=\"https://www.maptiler.com/copyright/\" target=\"_blank\"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e",
    crossOrigin: true
}).addTo(map);
var marker = L.marker([document.getElementById("Latitude").value, document.getElementById("Longitude").value], {
    draggable: false,

}).addTo(map);
map.dragging.disable();



