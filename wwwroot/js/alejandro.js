// 1. CONFIGURACIÓN INICIAL
// Nota: Asegúrate de que las constantes coincidan con tu panel de Supabase
const SUPABASE_URL = "https://tu-proyecto.supabase.co"; 
const SUPABASE_ANON_KEY = "tu-anon-key-aqui";
const CLIENT_ID = "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com";

// CORRECCIÓN: Usamos el objeto global 'supabase' de la librería externa
const _supabase = supabase.createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

// 2. FUNCIÓN PARA RECIBIR EL TOKEN DE GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Autenticando en Supabase...");

    const { data, error } = await _supabase.auth.signInWithIdToken({
        provider: 'google',
        token: response.credential,
    });

    if (error) {
        console.error("Error de Supabase:", error.message);
        alert("Error al vincular con Supabase: " + error.message);
    } else {
        console.log("¡Login exitoso!", data);
        window.location.href = "/Home/Index";
    }
}

// 3. INICIALIZACIÓN DE GOOGLE Y LOGIN MANUAL
window.onload = function () {
    // Inicializar Google
    if (document.getElementById("google-login-btn") || document.getElementById("google-register-btn")) {
        google.accounts.id.initialize({
            client_id: CLIENT_ID,
            callback: handleGoogleResponse
        });

        const btnId = document.getElementById("google-login-btn") ? "google-login-btn" : "google-register-btn";

        google.accounts.id.renderButton(
            document.getElementById(btnId),
            { theme: "outline", size: "large", width: "100%", text: "signin_with", shape: "pill" }
        );
    }

    // LÓGICA DEL LOGIN MANUAL
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            // Aquí NO ponemos e.preventDefault() si queremos que el formulario 
            // viaje a nuestro controlador de .NET de forma tradicional.
            console.log("Enviando login a .NET...");
        });
    }
};

// 4. EL GUARDIA DEL REGISTRO (Fuera del onload para que sea más rápido)
document.addEventListener("DOMContentLoaded", function () {
    const regForm = document.getElementById('registerForm');
    
    if (regForm) {
        regForm.addEventListener('submit', function (e) {
            const pass = document.getElementById('regPass').value;
            const confirm = document.getElementById('regPassConfirm').value;

            if (pass !== confirm) {
                e.preventDefault(); // Detiene el envío si están mal
                alert("¡Las contraseñas no coinciden, cawn! Chécalo bien.");
            } else {
                console.log("Contraseñas coinciden. Enviando a .NET...");
                // Aquí el formulario sigue su curso hacia ProcesarRegistro
            }
        });
    }
});