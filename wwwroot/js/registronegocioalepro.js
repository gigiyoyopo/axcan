document.addEventListener('DOMContentLoaded', function () {
    console.log("Iguano Power: Ubicación en tiempo real activada...");

    // 1. Configuración Inicial (CDMX por si falla el GPS)
    var latInicial = 19.4326;
    var lngInicial = -99.1332;
    
    var mapa = L.map('mapaRegistro').setView([latInicial, lngInicial], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap'
    }).addTo(mapa);

    var marcador = L.marker([latInicial, lngInicial], {
        draggable: true
    }).addTo(mapa);

    // --- FUNCIÓN MAESTRA: Obtener dirección desde Coordenadas (Geocoding Inverso) ---
    function obtenerDireccion(lat, lng) {
        fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lng}`)
            .then(response => response.json())
            .then(data => {
                if (data.display_name) {
                    document.getElementById('direccionEscrita').value = data.display_name;
                    console.log("Dirección actualizada:", data.display_name);
                }
            })
            .catch(err => console.error("Error al obtener dirección:", err));
        
        // Actualizar inputs ocultos para C#
        document.getElementById('latitud').value = lat;
        document.getElementById('longitud').value = lng;
    }

    // --- GEOLOCALIZACIÓN EN TIEMPO REAL AL CARGAR ---
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(function(position) {
            var userLat = position.coords.latitude;
            var userLng = position.coords.longitude;

            console.log("Ubicación real encontrada:", userLat, userLng);
            
            mapa.setView([userLat, userLng], 17);
            marcador.setLatLng([userLat, userLng]);
            obtenerDireccion(userLat, userLng);
        }, function(error) {
            console.warn("El usuario bloqueó el GPS o hubo un error:", error.message);
            obtenerDireccion(latInicial, lngInicial); // Fallback a CDMX
        });
    }

    // --- EVENTO: Actualizar al mover el marcador ---
    marcador.on('dragend', function () {
        var pos = marcador.getLatLng();
        obtenerDireccion(pos.lat, pos.lng);
    });

    // --- EVENTO: Actualizar al dar CLIC en cualquier parte del mapa ---
    mapa.on('click', function (e) {
        marcador.setLatLng(e.latlng);
        obtenerDireccion(e.latlng.lat, e.latlng.lng);
    });
});