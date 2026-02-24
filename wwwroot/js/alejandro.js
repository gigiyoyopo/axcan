// 1. CONFIGURACIÓN INICIAL
const SUPABASE_URL = "https://ulrgujabfocyvndutqen.supabase.co"; 
const SUPABASE_ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVscmd1amFiZm9jeXZuZHV0cWVuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzE2MzQ3NzUsImV4cCI6MjA4NzIxMDc3NX0.CoXZ4NNH41rst3CNXH5WC6e2kHBZX-o1AqHReg2VYXU";        
const CLIENT_ID = "1058925398660-p306po9ltjithuikrgpo7oapi3j7vmb1.apps.googleusercontent.com";

const supabase = supabase.createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

// 2. FUNCIÓN PARA RECIBIR EL TOKEN DE GOOGLE
async function handleGoogleResponse(response) {
    console.log("Token recibido de Google. Autenticando en Supabase...");

    const { data, error } = await supabase.auth.signInWithIdToken({
        provider: 'google',
        token: response.credential,
    });

    if (error) {
        console.error("Error de Supabase:", error.message);
        alert("Error al vincular con Supabase: " + error.message);
    } else {
        console.log("¡Login exitoso!", data);
        // Redirigir al Index de .NET
        window.location.href = "/Home/Index";
    }
}

// 3. INICIALIZACIÓN AL CARGAR LA PÁGINA
window.onload = function () {
    // Inicializar Google
    google.accounts.id.initialize({
        client_id: CLIENT_ID,
        callback: handleGoogleResponse
    });

    // Renderizar el botón en el div con id "google-login-btn"
    google.accounts.id.renderButton(
        document.getElementById("google-login-btn"),
        { 
            theme: "outline", 
            size: "large", 
            width: "100%",
            text: "signin_with",
            shape: "pill"
        }
    );

    // LÓGICA DEL FORMULARIO MANUAL (POSTGRESQL DIRECTO)
    const loginForm = document.getElementById('loginForm');
    if(loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            const user = document.getElementById('userInput').value;
            const pass = document.getElementById('passInput').value;

            console.log("Intentando login manual para:", user);
            
            // Aquí podrías usar supabase.auth.signInWithPassword si usas el Auth de Supabase
            // O un fetch a tu API de .NET si validas tú mismo en la BD
            alert("Validando credenciales en PostgreSQL...");
        });
    }
};