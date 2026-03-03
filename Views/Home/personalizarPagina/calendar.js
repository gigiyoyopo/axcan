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
                                   /|              /%@/             \°#\              |\
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


let lastCell = "body";


function load() {

    if(b == 0) {
        b++;
        createCurrentCalendar(index);
    }
}


function prev_month() {
    lastCell = "body";
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
    lastCell = "body";
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
            
            if(lastCell != targetId) {
                console.log(targetId);
            displayDate(stringArray);
            colorCell(targetId, lastCell);
            lastCell = targetId;
            }
            
        
    }
    
}


function displayDate(stringArray) {
    var dateString = "";

    const dateStringElement = document.getElementById("dateString");


    if(stringArray[2] == "0") {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[3] + " de " + months[index] + " de " + systemDate.getFullYear();
    } else {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[2] + stringArray[3] + " de " + months[index] + " de " + systemDate.getFullYear();
    }
    
    dateNumber = stringArray[2] + "" + stringArray[3] + "/" + (index + 1) + systemDate.getFullYear();

    dateStringElement.textContent = dateString;
    console.log(dateString);


    

    enable();

 
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

    const serviceSelection = document.getElementById("service-select");
    const hourSelection = document.getElementById("hour-select");
    const serverSelection = document.getElementById("server-select");

    serviceSelection.setAttribute("disabled", "true");
    hourSelection.setAttribute("disabled", "true");
    serverSelection.setAttribute("disabled", "true");
    accept.setAttribute("disabled", "true");

    const selectedOption = document.getElementById("selected-option");
    const selectedOption1 = document.getElementById("selected-option-1");
    const selectedOption2 = document.getElementById("selected-option-2");
    
    selectedOption.remove();
    selectedOption1.remove();
    selectedOption2.remove();

    //price.textContent = "Precio:";

    const newSelectedOption = document.createElement("option");
    const newSelectedOption1 = document.createElement("option");
    const newSelectedOption2 = document.createElement("option");

    const text = document.createTextNode("Selecciona un servicio");
    const text1 = document.createTextNode("Selecciona un horario");
    const text2 = document.createTextNode("Selecciona un servidor");

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

    if(a != 0) {

        serviceSelect.removeAttribute("disabled");
        a = 0;
    }
}







//defaultRadio.addEventListener("click", check());


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




const dateDisplay = document.getElementById("current-date");
dateDisplay.textContent = "Hoy: " + week[systemDay] + " " + systemDate.getDate() + " de " + months[index] + " del " + 2026;  

const prev = document.getElementById("prev_month");
prev.addEventListener("click", prev_month);

const next = document.getElementById("next_month");
next.addEventListener("click", next_month);

document.addEventListener("load", load());