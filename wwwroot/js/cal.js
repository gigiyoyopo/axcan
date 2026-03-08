/*
                                                             _
                                                            /^\
                                                           /%&#\
                                                          /%@$°#\
                                                         /%@/ \°#\
                                                        /%@/   \°#\
                                                       /%@/     \°#\
                                                      /%@/       \°#\
                                                     /%@/         \°#\
                                     +              /%@/           \°#\               +
                                   /|              /%@/             
                                   
                                   
                                   \°#\              |\
                                  /|:             /%@/               \°#\             :|\
                                 /#:             /%@/                 \°#\             :@\
                                /#:             /%@/                   \°#\             :@\
                               /#:             /%@/                     \°#\             :#\
                              /@:             /%@/                       \°#\             :#\
                             /@:             /%@/                         \°#\             :#\
                            (%$]            (+|]                           [|+)            [$%)
                            \@:              \°#\                         /%@/             :#/
                             \@:              \°#\                       /%@/             :#/
                              \@:              \°#\                     /%@/             :#/
                               \#:              \°#\                   /%@/             :@/
                                \#:              \°#\                 /%@/             :@/
                                 \#:              \°#\               /%@/             :@/
                                  \|:              \°#\             /%@/             :|/
                                   \|:              \°#\           /%@/             :|/
                                    \|               \°#\         /%@/              |/
                                     *                \°#\       /%@/               *
                                                       \°#\     /%@/
                                                        \°#\   /%@/
                                                         \°#\_/%@/
                                                          \°║▓║@/
                                                           *║▓║*
                                                            ║▓║
                                           _                ║▓║                _
                                          \°#\              ║▓║              /%$/
                                           \°#\             ║▓║             /%$/
                                            \°#\            ║▓║            /%$/
                                             \°#\           ║▓║           /%$/
                                              \°#\          ║▓║          /%$/
                                               \°#\         ║▓║         /%$/
                                                \°#\        ║▓║        /%$/
                                                 \°#\       ║▓║       /%$/
                                                  \°#\      ║▓║      /%$/
                                                   \°#\     ║▓║     /%$/
                                                    \°#\    ║▓║    /%$/
                                                     \°#\   ║▓║   /%$/
                                                      \°#\  ║▓║  /%$/
                                                       \°#\ ║▓║ /%$/
                                                        \°#\║▓║/%$/
                                   ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄║▓║▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
                                                        /%$/║▓║\°#\
                                                       /%$/ ║▓║ \°#\
                                                      /%$/  ║▓║  \°#\
                                                     /%$/   ║▓║   \°#\
                                                    /%$/    ║▓║    \°#\
                                                   /%$/     ║▓║     \°#\
                                                  /%$/      ║▓║      \°#\
                                                 /%$/       ║▓║       \°#\
                                                /%$/        ║▓║        \°#\
                                               /%$/         ║▓║         \°#\
                                              /%$/          ║▓║          \°#\
                                             /%$/           ║▓║           \°#\
                                            /%$/            ║▓║            \°#\
                                           /%$/             ║▓║             \°#\
                                          /%$/              ║▓║              \°#\
                                =========================================================
                                HBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHBHHBHBHBHBHBHBHB
                                =========================================================
                                            @@@@@@     @@@@@@    @@@@@@@@@@@
                                            @@@@@@     @@@@@@    @@@@@@@@@@@@
                                            @@@@@@     @@@@@@    @@@@@     @@@@
                                            @@@@@@     @@@@@@    @@@@@      @@@@
                                            @@@@@@     @@@@@@    @@@@@      @@@@
                                            @@@@@@@@@@@@@@@@@    @@@@@     @@@@
                                            @@@@@@@@@@@@@@@@@    @@@@@@@@@@@
                                            @@@@@@@@@@@@@@@@@    @@@@@@@@@@@@@
                                            @@@@@@     @@@@@@    @@@@@      @@@@@
                                            @@@@@@     @@@@@@    @@@@@       @@@@@
                                            @@@@@@     @@@@@@    @@@@@       @@@@@
                                            @@@@@@     @@@@@@    @@@@@     @@@@@
                                            @@@@@@     @@@@@@    @@@@@@@@@@@@@
                                            @@@@@@     @@@@@@    @@@@@@@@@@@
                                                          Hand  Bringer
                                                          ХАНД  БРИНГЭР
*/

let months = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
let matrixMonths = [[5, 31, 5], [1, 28, 4], [1, 31, 5], [4, 30, 5], [6, 31, 6], [2, 30, 5], [4, 31, 5], [7, 31, 6], [3, 30, 5], [5, 31, 5], [1, 30, 5], [3, 31, 5]]; 
//[inicio, dias, semanas]


let systemDate = new Date();
let systemMonth = systemDate.getMonth();
let systemDay = systemDate.getDay();
let week = ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sábado"];
let index = systemMonth;

let a = 0;
let b = 0;

function load() {

    if(b == 0) {
        b++;
        createCurrentCalendar(index);
    }
}


function prev_month() {
    index--;
    if(index <= systemMonth) {
        index = systemMonth;
    }    
    if(systemMonth == index) {
        createCurrentCalendar(index);
    } else {
        createCalendar(index);
    }
}


function next_month() {
    index++;
    if(index == 12) {
        index = 11;
    }
    if(systemMonth == index) {
        createCurrentCalendar(index);
    } else {
        createCalendar(index);
    }

    
}


function createCurrentCalendar(currentMonth) {

    const calendarContainer = document.getElementById("calendar-container");

    const calendar__table = document.getElementById("calendar-table");
    
    const calendarMonth = document.getElementById("month");
    const table = document.createElement("table");
    const thead = document.createElement("thead");
    const trDays = document.createElement("tr");

    const tbody = document.createElement("tbody");

    var i = 0;
    var j = 0;
    var k = 0;


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
        day = document.createTextNode(week[i]);
        thDays.appendChild(day);
    
    
    }


    table.appendChild(tbody);
    tbody.setAttribute("id", "calendar-numbers");


    for(i = 0; i < matrixMonths[currentMonth][2]; i++) {
  
        const trWeek = document.createElement("tr");
        tbody.appendChild(trWeek);
        trWeek.setAttribute("id", "week-row");


        for (j = 0; j < 7; j++) {
        
            const td = document.createElement("td");
            const p = document.createElement("p");
            trWeek.appendChild(td);
            td.setAttribute("id", "cl-" + (j + k + 1));
            td.appendChild(p);
            p.setAttribute("id", "cl-p-" + (j + k + 1));
            console.log("Día del sistema en for: " + systemDate.getDate());

            if(j + k - matrixMonths[currentMonth][0] <= matrixMonths[currentMonth][1] -2 
                && (j + k + 1) >= matrixMonths[currentMonth][0]) {
                    
                    if(j + k + 1>= systemDate.getDate()) {

                    document.getElementById("cl-" + (j + k + 1)).style.cursor = "pointer";
                if(((j + k + 2) - matrixMonths[currentMonth][0]) < 10) {
                    document.getElementById("cl-" + (j + k + 1)).setAttribute("id", j + "-0" + ((j + k + 2) - matrixMonths[currentMonth][0]) + "-available");
                    dayNumber = document.createTextNode((j + k + 2) - matrixMonths[currentMonth][0]);
                    p.appendChild(dayNumber);                        
                } else {
                    document.getElementById("cl-" + (j + k + 1)).setAttribute("id", j + "-" + ((j + k + 2) - matrixMonths[currentMonth][0]) + "-available");
                    dayNumber = document.createTextNode((j + k + 2) - matrixMonths[currentMonth][0]);
                    p.appendChild(dayNumber);
                }
                    } else {

                        document.getElementById("cl-" + (j + k + 1)).style.backgroundColor = "grey";
                        dayNumber = document.createTextNode((j + k + 2) - matrixMonths[currentMonth][0]);
                    p.appendChild(dayNumber);        
                    }
            } else {
                document.getElementById("cl-" + (j + k + 1)).style.backgroundColor = "grey";
            }
        }
        k = k + 7;
    }
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
    var j = 0;
    var k = 0;


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
        day = document.createTextNode(week[i]);
        thDays.appendChild(day);
    
    
    }


    table.appendChild(tbody);
    tbody.setAttribute("id", "calendar-numbers");


    for(i = 0; i < matrixMonths[currentMonth][2]; i++) {
  
        const trWeek = document.createElement("tr");
        tbody.appendChild(trWeek);
        trWeek.setAttribute("id", "week-row");


        for (j = 0; j < 7; j++) {
        
            const td = document.createElement("td");
            const p = document.createElement("p");
            trWeek.appendChild(td);
            td.setAttribute("id", "cl-" + (j + k + 1));
            td.appendChild(p);
            p.setAttribute("id", "cl-p-" + (j + k + 1));


            if(j + k - matrixMonths[currentMonth][0] <= matrixMonths[currentMonth][1] -2 
                && (j + k + 1) >= matrixMonths[currentMonth][0]) {
                document.getElementById("cl-" + (j + k + 1)).style.cursor = "pointer";
                if(((j + k + 2) - matrixMonths[currentMonth][0]) < 10) {
                    document.getElementById("cl-" + (j + k + 1)).setAttribute("id", j + "-0" + ((j + k + 2) - matrixMonths[currentMonth][0]) + "-available");
                    dayNumber = document.createTextNode((j + k + 2) - matrixMonths[currentMonth][0]);
                    p.appendChild(dayNumber);                        
                } else {
                    document.getElementById("cl-" + (j + k + 1)).setAttribute("id", j + "-" + ((j + k + 2) - matrixMonths[currentMonth][0]) + "-available");
                    dayNumber = document.createTextNode((j + k + 2) - matrixMonths[currentMonth][0]);
                    p.appendChild(dayNumber);
                }
            } else {
                document.getElementById("cl-" + (j + k + 1)).style.backgroundColor = "grey";
            }
        }
        k = k + 7;
    }
}


document.onclick = function checkDay(bool) {

    var targetId = bool.target.id;

    var stringArray = targetId.split("");

    if(a == 0 && stringArray[5] == "a") {

        console.log(targetId);
        openModal(stringArray);
    }
    
}


function openModal(stringArray) {
    var dateString = "";
    var dateNumber;

    const dateStringElement = document.getElementById("dateString");
    const dateNumberElement = document.getElementById("dateNumber");


    a++;
    if(stringArray[2] == "0") {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[3] + " de " + months[index] + " de 2026";
    } else {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[2] + stringArray[3] + " de " + months[index] + " de 2026";
    }
    
    dateNumber = stringArray[2] + "" + stringArray[3] + "/" + (index + 1) + "/2026";

    dateStringElement.textContent = dateString;
    dateNumberElement.textContent = dateNumber
    console.log(dateString);

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
prev.addEventListener("click", prev_month);

const next = document.getElementById("next_month");
next.addEventListener("click", next_month);

const close_Modal = document.getElementById("closeModal");
close_Modal.addEventListener("click", closeModal);

const close_Modal_Cancel = document.getElementById("cancel");
close_Modal_Cancel.addEventListener("click", closeModal);

document.addEventListener("load", load());