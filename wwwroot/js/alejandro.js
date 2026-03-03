// 1. CONFIGURACIÓN INICIAL (Limpia y sin duplicados)
const CLIENT_ID = "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com";

// Inicializamos Supabase
const supabaseClient = supabase.createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

// 2. FUNCIÓN PARA RECIBIR EL TOKEN DE GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Autenticando en Supabase...");

    const { data, error } = await supabaseClient.auth.signInWithIdToken({
        provider: 'google',
        token: response.credential,
    });

    if (error) {
        console.error("Error de Supabase:", error.message);
        alert("Error al vincular con Supabase: " + error.message);
    } else {
        console.log("¡Login en Supabase exitoso!", data);
        
        // Redirección temporal para validar visualmente el flujo
        window.location.href = "/Home/Index";
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