// 1. CONFIGURACIÓN INICIAL (Limpia y sin duplicados)
const CLIENT_ID = "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com";

// Inicializamos Supabase
const supabaseClient = supabase.createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

// 2. FUNCIÓN PARA RECIBIR EL TOKEN DE GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Autenticando...");

    // A. Registrar en Supabase (como ya lo tenías)
    const { data, error } = await supabaseClient.auth.signInWithIdToken({
        provider: 'google',
        token: response.credential,
    });

    if (error) {
        console.error("Error de Supabase:", error.message);
        alert("Error al vincular con Supabase: " + error.message);
        return; // Detenemos el proceso si Supabase falla
    } 
    
    console.log("¡Login en Supabase exitoso!", data);

    // B. EL NUEVO PUENTE: Enviar el token a .NET para crear la Cookie
    console.log("Avisando al servidor .NET...");
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
            console.log("¡.NET generó la cookie exitosamente! Redirigiendo...");
            // C. Ahora sí, cambiamos de página con permiso oficial
            window.location.href = resultadoNet.redirectUrl; 
        } else {
            alert("Error de seguridad en .NET: " + resultadoNet.message);
        }
    } catch (err) {
        console.error("Error conectando con C#:", err);
    }
}

// 3. INICIALIZACIÓN DE GOOGLE Y LOGIN MANUAL
window.onload = function () {
    // Escuchador para el formulario manual
    const loginForm = document.getElementById('loginForm');
    if(loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            console.log("Intentando login manual...");
            alert("Validando credenciales en PostgreSQL...");
        });
    }
});