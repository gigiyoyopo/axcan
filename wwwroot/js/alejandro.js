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

    // Solo ejecutamos esto si estamos en la página de registro
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const pass = document.getElementById('regPass').value;
            const passConfirm = document.getElementById('regPassConfirm').value;

            if (pass !== passConfirm) {
                e.preventDefault(); // Bloqueamos el envío
                alert("¡Las contraseñas no coinciden, cawn! Chécalo bien.");
                return;
            }

            // Si todo está bien, dejamos que el formulario llegue a C# sin decir nada extra.
        });
    }
};