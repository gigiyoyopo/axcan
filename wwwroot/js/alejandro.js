// ÚNICAMENTE LA FUNCIÓN DE GOOGLE
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
            console.error("El servidor rechazó el login:", resultadoNet.message);
            alert("Error de seguridad en .NET: " + resultadoNet.message);
        }
    } catch (err) {
        console.error("Error conectando con C#:", err);
        alert("Ocurrió un error al conectar con el servidor.");
    }
}