var map = null;
var latlngs = [[40.736504, -74.04515], [40.733138, -74.027472], [40.744642, -74.02224], [40.754143, -74.020006], [40.758141, -74.033501], [40.751087, -74.037942], [40.746161, -74.040624], [40.743202, -74.042769]];
var incidentMarker = L.icon({
    iconUrl: 'incident_marker.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowUrl: 'incident_marker_shadow.png',
    tooltipAnchor: [16, -28],
    shadowSize: [41, 41]
});
function mapinit() {
    map = L.map('map').setView([51.505, -0.09], 13);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    var popup = L.popup();

    function onMapClick(e) {
        popup
            .setLatLng(e.latlng)
            .setContent(e.latlng.lat + ", " + e.latlng.lng)
            .openOn(map);
    }
    var polygon = L.polygon(latlngs, { color: 'blue', fillOpacity:0.1, stroke:true, weight:1 }).addTo(map);

    // zoom the map to the polygon
    map.fitBounds(polygon.getBounds());
    map.on('click', onMapClick);
}

units_dictionary = {}
currentMapObjects = {}
calls = []
callMapObjects = []

function addLocation(unit, lat, long, ts) {
    units_dictionary[unit] = [lat, long, ts]
}

function addCallLocation(lat, long) {
    //console.log("lat" , lat, " long", long)
    calls.push([lat, long,])
}

function renderLocationObjects() {
    for (const [key, value] of Object.entries(units_dictionary)) {
        if (currentMapObjects[key] == null) {
            currentMapObjects[key] = L.marker([value[0], value[1]]).addTo(map);
        } else {
            currentMapObjects[key].setLatLng([value[0], value[1]]);
        }
        
        currentMapObjects[key].bindPopup('<strong>' + key + '</strong> <br>' + value[2])
    }

    for (const [key, value] of Object.entries(callMapObjects)) {
        //console.log("removing: ", key);
        value.remove()
    }
    callMapObjects = []
    for (const [key, value] of Object.entries(calls)) {
        callMapObjects[key] = L.marker([value[0], value[1]], { icon: incidentMarker }).addTo(map);
    }
    calls = []

}

