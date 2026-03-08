
let months = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
let matrixMonths = [[5, 31, 5], [1, 28, 4], [1, 31, 5], [4, 30, 5], [6, 31, 6], [2, 30, 5], [4, 31, 5], [7, 31, 6], [3, 30, 5], [5, 31, 5], [1, 30, 5], [3, 31, 5]]; 
//[inicio, dias, semanas]


let sysDate = new Date();
// /*
let systemDay = sysDate.getDay(); 
let systemDate = sysDate.getDate();
let systemMonth = sysDate.getMonth();
// */
 /*
let systemDay = 0;
let systemDate = 1;
let systemMonth = 2;
// */
let week = ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado","Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado"];

let index = systemMonth;

let a = 0; //Variable para el modal
let b = 0; //Variable para iniciar una ves el script cuando la página carga

let dayAcc = systemDate + 6;
let accCopy = dayAcc;

let maxPrev = dayAcc;



function load() {

    if(b == 0) {
        b++;
        createCalendar(index);
    }
}



function prev_week(weekNum) {
    
    dayAcc = dayAcc - 7;
    var x = 6 - systemDay;
    

    if(dayAcc <= accCopy && index == systemMonth) {
        dayAcc = accCopy;
    }

    if(dayAcc <= 0) {
        dayAcc = maxPrev - 7;
        index--;
    }

    if(systemDay >= week.length) {
        systemDay = -x;
    }

    if(index == -1) {
        index = 0;
    }


    createCalendar(index);
}



function next_week() {

    dayAcc+=7;
    var x = 6 - systemDay;


    if(systemDay >= week.length) {
        systemDay = -x;
    }

    if(dayAcc - 7>= matrixMonths[index][1]) {
        maxPrev = dayAcc;
        index++;
        dayAcc = systemDay - matrixMonths[index][0] + 1;
        
        if(dayAcc < 0) {
            dayAcc = dayAcc + 7;
        }

        if(dayAcc == 0) {
            dayAcc = 7;
        }

        if(dayAcc > 7) {
            dayAcc = x;
        }    

    }


    if(index == 12) {
        index = 11;
    }


    createCalendar(index);
}



function createCalendar(currentMonth) {

    const calendarContainer = document.getElementById("calendar-container");

    const calendar__table = document.getElementById("calendar-table");
    
    const calendarMonth = document.getElementById("month");
    const table = document.createElement("table");
    const thead = document.createElement("thead");
    const trDays = document.createElement("tr");

    const tbody = document.createElement("tbody");

    var i = 0;


    calendar__table.remove();

    calendarMonth.textContent = months[currentMonth];
    calendarContainer.appendChild(table);
    table.setAttribute("id", "calendar-table");

    table.appendChild(thead);
    thead.setAttribute("id", "calendar-header");

    thead.appendChild(trDays);
    trDays.setAttribute("id", "calendar-days");

    
    for(i = 0; i < 7; i++) {
        
        const thDays = document.createElement("th");
        trDays.appendChild(thDays);
        day = document.createTextNode(week[systemDay + i]);
        thDays.appendChild(day);
    }

    table.appendChild(tbody);
    tbody.setAttribute("id", "calendar-numbers");

        const trWeek = document.createElement("tr");
        tbody.appendChild(trWeek);
        trWeek.setAttribute("id", "week-row");
        for(i = 0; i < 7; i++) {
            const td = document.createElement("td");
            const p = document.createElement("p");
            var dayNumber = "";
            var localDayAcc;

            trWeek.appendChild(td);
            localDayAcc = ((i + dayAcc - 6));
            td.setAttribute("id", "cl-" + (localDayAcc));
            td.appendChild(p);
            p.setAttribute("id", "cl-p-" + (localDayAcc));
            if(localDayAcc <= matrixMonths[currentMonth][1] && localDayAcc >= 1) {

                document.getElementById("cl-" + (localDayAcc)).style.cursor = "pointer";
                if(localDayAcc < 10) {
                    document.getElementById("cl-" + (localDayAcc)).setAttribute("id", (i + systemDay) + "-0" + (localDayAcc) + "-a");
                    dayNumber = document.createTextNode(localDayAcc);
                    p.appendChild(dayNumber);
                } else {
                    document.getElementById("cl-" + (localDayAcc)).setAttribute("id", (i + systemDay) + "-"+ (localDayAcc) + "-a");
                    dayNumber = document.createTextNode(localDayAcc);
                    p.appendChild(dayNumber);
                }
                
            } else {
                document.getElementById("cl-" + (localDayAcc)).style.backgroundColor = "grey";
            }
            
        }
}



document.onclick = function checkDay(bool) {

    var targetId = bool.target.id;

    var stringArray = targetId.split("");

    if(stringArray.length == 6) {
        if(a == 0 && stringArray[5] == "a") {
            openModal(stringArray);
        }
    } else if (stringArray.length == 7) {
        if(a == 0 && stringArray[6] == "a") {
            openModal(stringArray);
        }
    }
    
}



function openModal(stringArray) {
    var dateString = "";
    var dateNumber;

    const dateStringElement = document.getElementById("dateString");
    const dateNumberElement = document.getElementById("dateNumber");


    a++;

    if(stringArray.length == 6) {
        if(stringArray[2] == "0") {
            dateString = dateString + week[stringArray[0]] + " " + stringArray[3] + " de " + months[index] + " de 2026";
        } else {
            dateString = dateString + week[stringArray[0]] + " " + stringArray[2] + stringArray[3] + " de " + months[index] + " de 2026";
        }
    
        dateNumber = stringArray[2] + "" + stringArray[3] + "/" + (index + 1) + "/2026";
    } else if(stringArray.length == 7) {
        if(stringArray[2] == "0") {
            dateString = dateString + week[stringArray[0] + stringArray[1] + ""] + " " + stringArray[3] + " de " + months[index] + " de 2026";
        } else {
            dateString = dateString + week[stringArray[0] + stringArray[1] + ""] + " " + stringArray[3] + stringArray[4] + " de " + months[index] + " de 2026";
        }
    
        dateNumber = stringArray[3] + "" + stringArray[4] + "/" + (index + 1) + "/2026";

    }
    

    dateStringElement.textContent = dateString;
    dateNumberElement.textContent = dateNumber

    document.getElementById("modal").style.display = "block";
 
}



function closeModal() {
    a--;
    document.getElementById("modal").style.display = "none";

    const confirmButton = document.getElementById("confirm");
    const price = document.getElementById("price");

    const serviceSelection = document.getElementById("service");
    const hourSelection = document.getElementById("hour");
    const serverSelection = document.getElementById("server");

    hourSelection.setAttribute("disabled", "true");
    serverSelection.setAttribute("disabled", "true");
    confirmButton.setAttribute("disabled", "true");

    const selectedOption = document.getElementById("selected-option");
    const selectedOption1 = document.getElementById("selected-option-1");
    const selectedOption2 = document.getElementById("selected-option-2");
    
    selectedOption.remove();
    selectedOption1.remove();
    selectedOption2.remove();

    price.textContent = "Precio:";

    const newSelectedOption = document.createElement("option");
    const newSelectedOption1 = document.createElement("option");
    const newSelectedOption2 = document.createElement("option");

    const text = document.createTextNode("Selecciona una opción");
    const text1 = document.createTextNode("Selecciona una opción");
    const text2 = document.createTextNode("Selecciona una opción");

    serviceSelection.appendChild(newSelectedOption);
    hourSelection.appendChild(newSelectedOption1);
    serverSelection.appendChild(newSelectedOption2);
    
    newSelectedOption.setAttribute("id", "selected-option");
    newSelectedOption.setAttribute("value", "null");
    newSelectedOption.setAttribute("selected", "true");
    newSelectedOption.setAttribute("disabled", "true");

    newSelectedOption1.setAttribute("id", "selected-option-1");
    newSelectedOption1.setAttribute("value", "null");
    newSelectedOption1.setAttribute("selected", "true");
    newSelectedOption1.setAttribute("disabled", "true");

    newSelectedOption2.setAttribute("id", "selected-option-2");
    newSelectedOption2.setAttribute("value", "null");
    newSelectedOption2.setAttribute("selected", "true");
    newSelectedOption2.setAttribute("disabled", "true");
    
    
    newSelectedOption.appendChild(text);
    newSelectedOption1.appendChild(text1);
    newSelectedOption2.appendChild(text2);
}



let serviceSelect = document.getElementById("service");
let hourSelect = document.getElementById("hour");
let serverSelect = document.getElementById("server");
let formButton = document.getElementById("confirm");



serviceSelect.addEventListener("change", () => {
    
    if(serviceSelect.value != "null") {
        hourSelect.removeAttribute("disabled");
    }
});


hourSelect.addEventListener("change", () => {
    
    if(hourSelect.value != "null") {
        serverSelect.removeAttribute("disabled");
    }
});


serverSelect.addEventListener("change", () => {
    
    if(hourSelect.value != "null") {
        formButton.removeAttribute("disabled");
        const price = document.getElementById("price");
        var precio = (Math.random() * 1000).toFixed(2);
        price.textContent = "Precio: $" + precio;
    }
});



const prev = document.getElementById("prev_month");
prev.addEventListener("click", prev_week);

const next = document.getElementById("next_month");
next.addEventListener("click", next_week);

const close_Modal = document.getElementById("closeModal");
close_Modal.addEventListener("click", closeModal);

const close_Modal_Cancel = document.getElementById("cancel");
close_Modal_Cancel.addEventListener("click", closeModal);

document.addEventListener("load", load());
// Agrega esto al final de wCal.js
async function agendarCitaFinal() {
    const tel = document.getElementById('telCliente').value;
    const esOtro = document.getElementById('checkAlguienMas').checked;
    const nombreOtro = document.getElementById('nombreAlguienMas').value;

    // Validación de 10 dígitos (Punto 5 de tu lista)
    if (tel.length !== 10 || isNaN(tel)) {
        alert("El teléfono debe ser de 10 dígitos numéricos.");
        return;
    }

    const formData = new FormData();
    // Sacamos los datos de los IDs que ya tienes en wCal.js
    formData.append("id_usuario_tramito", document.getElementById('UsuarioIdHidden').value);
    formData.append("id_empresa", document.getElementById('idEmpresaHidden').value);
    formData.append("fecha", document.getElementById('fecha_seleccionada_oculta').value); 
    formData.append("hora", document.getElementById('hour-select').value);
    formData.append("tipo_servicio", document.getElementById('service-select').value);
    formData.append("quien_atiende", document.getElementById('server-select').value);
    
    // Parámetros de validación para el HomeController
    formData.append("telefonoCliente", tel);
    formData.append("esParaAlguienMas", esOtro);
    formData.append("nombreAlguienMas", nombreOtro);

    const resp = await fetch('/Home/AgendarCita', { method: 'POST', body: formData });
    const res = await resp.json();

    if(res.success) {
        alert("¡Iguano Exitoso! " + res.mensaje);
        location.reload();
    } else {
        alert("Error: " + res.mensaje);
    }
}



