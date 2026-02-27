document.addEventListener('DOMContentLoaded', function () {
    console.log("Iguano Power activado: Cargando mapa y validaciones...");

    // --- 1. CONFIGURACIÓN DEL MAPA (Leaflet) ---
    var latInicial = 19.4326;
    var lngInicial = -99.1332;
    
    // Inicializar el mapa
    var mapa = L.map('mapaRegistro').setView([latInicial, lngInicial], 13);

    // Agregar capa de OpenStreetMap
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap'
    }).addTo(mapa);

    // Marcador rojo movible
    var marcador = L.marker([latInicial, lngInicial], {
        draggable: true
    }).addTo(mapa);

    // Función para pasar coordenadas a los inputs ocultos
    function actualizarCoords(lat, lng) {
        const inputLat = document.getElementById('latitud');
        const inputLng = document.getElementById('longitud');
        if(inputLat && inputLng){
            inputLat.value = lat;
            inputLng.value = lng;
        }
        console.log("Ubicación fijada:", lat, lng);
    }

    // Eventos del marcador y clic
    marcador.on('dragend', function () {
        var pos = marcador.getLatLng();
        actualizarCoords(pos.lat, pos.lng);
    });

    mapa.on('click', function (e) {
        marcador.setLatLng(e.latlng);
        actualizarCoords(e.latlng.lat, e.latlng.lng);
    });

    // --- 2. LÓGICA DEL BUSCADOR ---
    const btnBuscar = document.getElementById('btnBuscar');
    if (btnBuscar) {
        btnBuscar.addEventListener('click', function () {
            var direccion = document.getElementById('buscadorUbi').value;
            if (direccion.length < 5) return alert("Escribe una dirección más clara, cawn.");

            fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${direccion}`)
                .then(response => response.json())
                .then(data => {
                    if (data.length > 0) {
                        var lat = data[0].lat;
                        var lon = data[0].lon;
                        mapa.setView([lat, lon], 16);
                        marcador.setLatLng([lat, lon]);
                        actualizarCoords(lat, lon);
                        document.getElementById('direccionEscrita').value = data[0].display_name;
                    } else {
                        alert("No encontré ese lugar.");
                    }
                });
        });
    }

    // --- 3. VALIDACIÓN DEL FORMULARIO ---
    const form = document.getElementById("formRegistroNegocio");
    if (form) {
        form.addEventListener("submit", function (e) {
            const nombre = document.getElementById("nombreNegocio").value;
            const giro = document.getElementById("tipoServicio").value;
            const lat = document.getElementById("latitud").value;

            // Validar campos básicos
            if (nombre.trim() === "" || giro.trim() === "") {
                e.preventDefault();
                alert("¡Falta el nombre o el giro del negocio!");
                return;
            }

            // Validar que marcaron ubicación
            if (lat === "") {
                e.preventDefault();
                alert("Por favor, marca la ubicación del negocio en el mapa.");
                return;
            }

            console.log("Formulario listo para enviar a C#...");
        });
    }
});