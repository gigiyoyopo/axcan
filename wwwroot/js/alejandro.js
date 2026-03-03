// 1. CONFIGURACIÓN INICIAL (Limpia y sin duplicados)
const CLIENT_ID = "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com";
const SUPABASE_URL = "https://ulrgujabfocyvndutqen.supabase.co"; 
const SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVscmd1amFiZm9jeXZuZHV0cWVuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzE2MzQ3NzUsImV4cCI6MjA4NzIxMDc3NX0.CoXZ4NNH41rst3CNXH5WC6e2kHBZX-o1AqHReg2VYXU";        

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

// 3. INICIALIZACIÓN AL CARGAR LA PÁGINA
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
};