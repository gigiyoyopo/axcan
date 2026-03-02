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
        displayDate(stringArray);
    }
    
}


function displayDate(stringArray) {
    var dateString = "";

    const dateStringElement = document.getElementById("dateString");


    if(stringArray[2] == "0") {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[3] + " de " + months[index] + " de 2026";
    } else {
        dateString = dateString + week[stringArray[0]] + " " + stringArray[2] + stringArray[3] + " de " + months[index] + " de 2026";
    }
    
    dateNumber = stringArray[2] + "" + stringArray[3] + "/" + (index + 1) + "/2026";

    dateStringElement.textContent = dateString;
    console.log(dateString);

 
}



function enable() {

    
}


const dateDisplay = document.getElementById("current-date");
dateDisplay.textContent = "Hoy: " + week[systemDay] + " " + systemDate.getDate() + " de " + months[index] + " del " + 2026;  

const prev = document.getElementById("prev_month");
prev.addEventListener("click", prev_month);

const next = document.getElementById("next_month");
next.addEventListener("click", next_month);

document.addEventListener("load", load());