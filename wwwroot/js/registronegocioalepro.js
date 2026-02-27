document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formRegistroNegocio");

    if (form) {
        form.addEventListener("submit", function (e) {
            
            const nombre = document.getElementById("nombreNegocio").value;
            const giro = document.getElementById("tipoServicio").value;
            const color = document.getElementById("colorIdentidad").value;
            const icono = document.getElementById("iconoNegocio").files[0];
// Esperar a que cargue la página
document.addEventListener('DOMContentLoaded', function () {
    // 1. Inicializar Mapa (Centrado en México por defecto)
    var latInicial = 19.4326;
    var lngInicial = -99.1332;
    
    var mapa = L.map('mapaRegistro').setView([latInicial, lngInicial], 13);

    // 2. Agregar capa de diseño (OpenStreetMap)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap'
    }).addTo(mapa);

    // 3. Agregar el Marcador Rojo (Movible)
    var marcador = L.marker([latInicial, lngInicial], {
        draggable: true
    }).addTo(mapa);

    // Función para actualizar coordenadas en los inputs
    function actualizarCoords(lat, lng) {
        document.getElementById('latitud').value = lat;
        document.getElementById('longitud').value = lng;
        console.log("Coords actualizadas:", lat, lng);
    }

    // Al mover el marcador con el mouse
    marcador.on('dragend', function (event) {
        var posicion = marcador.getLatLng();
        actualizarCoords(posicion.lat, posicion.lng);
    });

    // Al hacer clic en cualquier parte del mapa
    mapa.on('click', function (e) {
        marcador.setLatLng(e.latlng);
        actualizarCoords(e.latlng.lat, e.latlng.lng);
    });

    // 4. Lógica del Buscador (Geocoding gratuito)
    document.getElementById('btnBuscar').addEventListener('click', function () {
        var direccion = document.getElementById('buscadorUbi').value;
        if (direccion.length < 5) return alert("Escribe una dirección más clara, cawn.");

        fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${direccion}`)
            .then(response => response.json())
            .then(data => {
                if (data.length > 0) {
                    var resultado = data[0];
                    var lat = resultado.lat;
                    var lon = resultado.lon;

                    mapa.setView([lat, lon], 16);
                    marcador.setLatLng([lat, lon]);
                    actualizarCoords(lat, lon);
                    document.getElementById('direccionEscrita').value = resultado.display_name;
                } else {
                    alert("No encontré ese lugar, intenta ser más específico.");
                }
            });
    });

    // Cargar libreria al final
});
            
            if (nombre.trim() === "" || giro.trim() === "") {
                e.preventDefault(); // Detiene el envío
                alert("Por favor, completa el nombre y el giro del negocio.");
                return;
            }

           
            if (icono) {
                const extensionesPermitidas = ['image/jpeg', 'image/png', 'image/jpg'];
                if (!extensionesPermitidas.includes(icono.type)) {
                    e.preventDefault();
                    alert("Por favor, sube una imagen válida (JPG o PNG).");
                    return;
                }
            }

       
            console.log(`Registrando: ${nombre} con el color: ${color}`);
        });
    }
});