// 1. FUNCIÓN PARA RECIBIR EL TOKEN DE GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Enviando al backend de Axcan (.NET)...");

    try {
        // Le enviamos el token a nuestra puerta de seguridad en C#
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
            // C# nos dio luz verde, vamos al panel
            window.location.href = resultadoNet.redirectUrl; 
        } else {
            console.error("El servidor rechazó el login:", resultadoNet.message);
            alert("Error de seguridad en .NET: " + resultadoNet.message);
        }
    } catch (err) {
        console.error("Error conectando con C#:", err);
        alert("Ocurrió un error al conectar con el servidor.");
    }
}

// 2. LÓGICA DEL FORMULARIO MANUAL (Opcional, por si lo usas después)
window.onload = function () {
    const loginForm = document.getElementById('loginForm');
    if(loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            // Aquí en el futuro puedes hacer un fetch a tu login manual de C#
            console.log("Intentando login manual...");
        });
    }
};