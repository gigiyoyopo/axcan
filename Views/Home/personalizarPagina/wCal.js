
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
let systemDay = 4;
let systemDate = 26;
let systemMonth = 11;
// */
let week = ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado","Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado"];

let index = systemMonth;

let a = 0; //Variable para el modal
let b = 0; //Variable para iniciar una ves el script cuando la página carga


let lastCell = "body";


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

    lastCell = "body";
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

    lastCell = "body";
    dayAcc+=7;
    var x = 6 - systemDay;


    if(systemDay >= week.length) {
        systemDay = -x;
    }

    if(dayAcc - 7>= matrixMonths[index][1]) {
        maxPrev = dayAcc;
        index++;

       // console.log("Index: " + index);
            //console.log("dayAcc: " + dayAcc);
        if(index == 12 && dayAcc - 7>= matrixMonths[index][1]) {
            const a = document.getElementById("next_month");
            a.setAttribute("disabled");
        }


        //console.log("Index after: " + index);
        //console.log("dayAcc after: " + dayAcc);
        
        
        dayAcc = systemDay - matrixMonths[index][0] + 1;
        
        if(dayAcc < 0) {
            dayAcc = dayAcc + 7;
        } else if(dayAcc == 0) {
            dayAcc = 7;
        } else if(dayAcc > 7) {
            dayAcc = x;
        }
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
           // openModal(stringArray);
            if(lastCell != targetId) {
                console.log(targetId);
            displayDate(stringArray);
            colorCell(targetId, lastCell);
            lastCell = targetId;
            }
        }
    } else if (stringArray.length == 7) {
        if(a == 0 && stringArray[6] == "a") {
            //openModal(stringArray);
            if(lastCell != targetId) {
                console.log(targetId);
            displayDate(stringArray);
            colorCell(targetId, lastCell);
            lastCell = targetId;
            }
        }
    }
    
}


function displayDate(stringArray) {
    var dateString = "";
    var dateNumber = "";

    const dateStringElement = document.getElementById("dateString");
    const idEmpresa = document.getElementById("idEmpresaHidden").value; // <--- Asegúrate de tener este input hidden en tu HTML

    // Lógica para armar la fecha según el ID del elemento clicado
    if(stringArray.length == 6) {
        let diaNum = stringArray[2] == "0" ? stringArray[3] : stringArray[2] + stringArray[3];
        dateString = week[stringArray[0]] + " " + diaNum + " de " + months[index] + " de 2026";
        dateNumber = `2026-${index + 1}-${diaNum}`; // Formato ISO para C#
    } else if(stringArray.length == 7) {
        let diaNum = stringArray[3] + stringArray[4];
        let diaSemanaIndex = parseInt(stringArray[0] + stringArray[1]);
        dateString = week[diaSemanaIndex] + " " + diaNum + " de " + months[index] + " de 2026";
        dateNumber = `2026-${index + 1}-${diaNum}`;
    }

    dateStringElement.textContent = dateString;
    
    // --- AQUÍ CONECTAMOS CON TU C# ---
    const hourSelect = document.getElementById("hour-select");
    hourSelect.innerHTML = '<option disabled selected>Buscando horarios...</option>';
    hourSelect.disabled = true;

    fetch(`/Home/GetHorariosDisponibles?idEmpresa=${idEmpresa}&fecha=${dateNumber}`)
        .then(res => res.json())
        .then(data => {
            enable(); // Reinicia los selects (tu función original)
            hourSelect.innerHTML = ''; // Limpiamos el "Buscando..."

            if (data.disponibles && data.disponibles.length > 0) {
                // Si hay horarios, los metemos al select
                data.disponibles.forEach(hora => {
                    let opt = document.createElement("option");
                    opt.value = hora;
                    opt.text = hora;
                    hourSelect.appendChild(opt);
                });
                // NO lo habilitamos todavía, hasta que elijan servicio (siguiendo la lógica de Axel)
            } else {
                let opt = document.createElement("option");
                opt.text = data.mensaje || "Sin cupo";
                hourSelect.appendChild(opt);
            }
        })
        .catch(err => {
            console.error("Error al cargar horarios:", err);
            hourSelect.innerHTML = '<option>Error al cargar</option>';
        });
}


function colorCell(targetId, lastCell) {

    const lastCellR = document.getElementById(lastCell);
    const selectedCell = document.getElementById(targetId);

    console.log("colorCell: " + targetId);
    selectedCell.style.backgroundColor = "grey";

    console.log("lastCellR: " + lastCell);
    lastCellR.style.backgroundColor = "white";
    
}




let hourSelect = document.getElementById("hour-select");

let serviceSelect = document.getElementById("service-select");

let serverSelect = document.getElementById("server-select");

let accept = document.getElementById("confirm");

function enable() {
    a++;
    // Referencias a los selects
    const serviceSelection = document.getElementById("service-select");
    const hourSelection = document.getElementById("hour-select");
    const serverSelection = document.getElementById("server-select");

    // Reset de estados
    serviceSelection.disabled = false; // El servicio siempre debe habilitarse al cambiar de día
    hourSelection.disabled = true;
    serverSelection.disabled = true;
    accept.disabled = true;

    // Limpiamos los textos de ayuda (Placeholders)
    limpiarSelect(serviceSelection, "Selecciona un servicio", "selected-option");
    limpiarSelect(hourSelection, "Selecciona un horario", "selected-option-1");
    limpiarSelect(serverSelection, "Selecciona un servidor", "selected-option-2");

    if(a != 0) {
        a = 0;
    }
}

// Función auxiliar para no repetir código
function limpiarSelect(elemento, texto, id) {
    elemento.innerHTML = "";
    const opt = document.createElement("option");
    opt.id = id;
    opt.value = "null";
    opt.selected = true;
    opt.disabled = true;
    opt.text = texto;
    elemento.appendChild(opt);
}
serviceSelect.addEventListener("change", () => {
    console.log("paso 1");
    hourSelect.removeAttribute("disabled");
});


hourSelect.addEventListener("change", () => {
    console.log("paso 2");
    serverSelect.removeAttribute("disabled");
});

serverSelect.addEventListener("change", () => {
    console.log("paso 3");
    accept.removeAttribute("disabled");
});

const prev = document.getElementById("prev_month");
prev.addEventListener("click", prev_week);

const next = document.getElementById("next_month");
next.addEventListener("click", next_week);


document.addEventListener("load", load());




// Cerrar con el botón "Cancelar" o la "X"
function cerrarTodo() {
    // Si usas el modal de Axel:
    const modal = document.getElementById("modal");
    if(modal) modal.style.display = "none";
    
    // Si solo quieres resetear colores
    if(lastCell != "body") {
        document.getElementById(lastCell).style.backgroundColor = "white";
        lastCell = "body";
    }
}

document.getElementById("cancel").addEventListener("click", cerrarTodo);
document.getElementById("closeModal").addEventListener("click", cerrarTodo);