// 1. FUNCIÓN MAESTRA PARA LOGIN Y REGISTRO CON GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Enviando al backend de Axcan (.NET)...");

    try {
        const resBackend = await fetch('/Home/AutenticarConGoogle', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ credential: response.credential })
        });

        const resultadoNet = await resBackend.json();

        if (resultadoNet.success) {
            console.log("¡.NET te dio el pase de entrada! Redirigiendo...");
            window.location.href = resultadoNet.redirectUrl; 
        } else {
            console.error("El servidor rechazó el login/registro:", resultadoNet.message);
            alert("Error en .NET: " + resultadoNet.message);
        }
    } catch (err) {
        console.error("Error conectando con C#:", err);
        alert("Ocurrió un error al conectar con el servidor.");
    }
}

// 2. VALIDACIÓN DEL FORMULARIO DE REGISTRO MANUAL
window.onload = function () {
    const registerForm = document.getElementById('registerForm');

    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const pass = document.getElementById('regPass').value;
            const passConfirm = document.getElementById('regPassConfirm').value;

            // Validación 1: Que coincidan
            if (pass !== passConfirm) {
                e.preventDefault(); 
                alert("Las contraseñas no coinciden. Por favor, verifícalas.");
                return;
            }

            // Seguridad de la contraseña (Mayúscula, Número, Símbolo, Min 8 caracteres)
            const passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/;
            if (!passwordRegex.test(pass)) {
                e.preventDefault();
                alert("Tu contraseña debe tener al menos 8 caracteres, incluir una mayúscula, un número y un símbolo especial (ej. @, #, $, !).");
                return;
            }

           
        });
    }
};