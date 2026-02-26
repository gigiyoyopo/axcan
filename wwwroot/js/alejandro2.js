const CLIENT_ID = "1058925398660-ovi8nq4pj2a0qtn7kmelganfug5lu008.apps.googleusercontent.com";

function handleGoogleRegister(response) {
    console.log("Token de Google recibido");
    // Aquí podrías hacer un fetch manual a una API, 
    // pero por ahora enfoquémonos en el formulario manual.
    alert("Google vinculado. Falta terminar el registro manual.");
}

window.onload = function () {
    google.accounts.id.initialize({
        client_id: CLIENT_ID,
        callback: handleGoogleRegister
    });

    google.accounts.id.renderButton(
        document.getElementById("google-register-btn"),
        { theme: "outline", size: "large", width: "100%", text: "signup_with" }
    );

    const registerForm = document.getElementById('registerForm');

    registerForm.addEventListener('submit', function(e) {
        // 1. Extraer valores (Usando las IDs corregidas)
        const pass = document.getElementById('regPass').value;
        const passConfirm = document.getElementById('regPassConfirm').value;

        // 2. Validar contraseñas
        if (pass !== passConfirm) {
            e.preventDefault(); // AQUÍ SÍ bloqueamos porque hay error
            alert("¡Las contraseñas no coinciden, cawn! Chécalo bien.");
            return;
        }

        // 3. SI TODO ESTÁ BIEN: 
        // No llamamos a e.preventDefault(). 
        // Dejamos que el formulario viaje a 'ProcesarRegistro' en C#.
        alert("¡Todo listo! Mandando datos a la base de datos...");
    });
};