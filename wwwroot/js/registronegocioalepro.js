document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formRegistroNegocio");

    if (form) {
        form.addEventListener("submit", function (e) {
            
            const nombre = document.getElementById("nombreNegocio").value;
            const giro = document.getElementById("tipoServicio").value;
            const color = document.getElementById("colorIdentidad").value;
            const icono = document.getElementById("iconoNegocio").files[0];

            
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