const CLIENT_ID = "1058925398660-p306po9ltjithuikrgpo7oapi3j7vmb1.apps.googleusercontent.com";


function handleGoogleRegister(response) {
    console.log("Datos de Google recibidos para nuevo registro");
    
    // El 'credential' es un token JWT que contiene el nombre y correo del usuario
    const payload = {
        token: response.credential,
        tipo_accion: 'registro_social', // guia para distinguir que hiso el wey del usuario para el  backend
        metodo: 'google'
    };

    console.log("Enviando token a PostgreSQL vía Backend:", payload);
    
    // Simulación de éxito
    alert("Registro con Google exitoso. Datos listos para la base de datos.");
}


window.onload = function () {
    
    google.accounts.id.initialize({
        client_id: CLIENT_ID,
        callback: handleGoogleRegister
    });

   
    google.accounts.id.renderButton(
        document.getElementById("google-register-btn"),
        { 
            theme: "outline", 
            size: "large", 
            width: "100%",
            text: "signup_with"
        }
    );

    //ACA KATO PORFAVOR CHECA LOS DATOS QUE SI ESTEN ENTRANDO BIEN PARO
    const registerForm = document.getElementById('registerForm');

    registerForm.addEventListener('submit', function(e) {
        e.preventDefault(); // Evita que la página se recargue

        //  valores de los inputs
        const nombre = document.getElementById('regName').value;
        const correo = document.getElementById('regEmail').value;
        const usuario = document.getElementById('regUser').value;
        const pass = document.getElementById('regPass').value;
        const passConfirm = document.getElementById('regPassConfirm').value;

        // Comparar contraseñas
        if (pass !== passConfirm) {
            alert("Las contraseñas no coinciden");
            return; // Detiene el proceso si fallan
        }

        // Estructura para pasar a el postgresito
        const datosUsuario = {
            full_name: nombre,
            email: correo,
            username: usuario,
            password: pass,// (Hash) PONER ACA 
            fecha_registro: new Date().toISOString()
        };

        console.log("Enviando nuevo usuario a PostgreSQL:", datosUsuario);
        
      
        alert("¡Registro enviado! Revisando disponibilidad en la base de datos...");
    });
};