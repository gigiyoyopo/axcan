// CONFIGURACIÓN

const CLIENT_ID = "1058925398660-p306po9ltjithuikrgpo7oapi3j7vmb1.apps.googleusercontent.com";

// token de Google
function handleGoogleResponse(response) {
    console.log("Sesión iniciada con Google");
    
    //  validar en la base de datos
    const payload = {
        token: response.credential,
        provider: 'google'
    };

    console.log("Datos para enviar al Backend:", payload);
    // Aquí irá tu fetch('/api/login-google', ...)
}

window.onload = function () {
    //  Inicializar Google Auth
    google.accounts.id.initialize({
        client_id: CLIENT_ID,
        callback: handleGoogleResponse
    });

    // botón de Google
    google.accounts.id.renderButton(
        document.getElementById("google-login-btn"),
        { theme: "outline", size: "large", width: "100%" }
    );

    //  Formulario Manual
    const loginForm = document.getElementById('loginForm');
    
    loginForm.addEventListener('submit', function(e) {
        e.preventDefault();

        // Recolectar 
        const user = document.getElementById('userInput').value;
        const pass = document.getElementById('passInput').value;

        // KATTO SE ESTA HACIENDO PENDEJO CON LA BD
        const loginData = {
            username: user,
            password: pass
        };

        console.log("Enviando credenciales a la base de datos:", loginData);
        
        // Simulasao
        alert("Validando datos en PostgreSQL...");
    });
};