

let mod1 = document.getElementById("mod1Select");
mod1.addEventListener("change", () => {
    
    const mod = document.getElementById("mod1Select").value;
    const iFrame = document.getElementById("iFrameMod1");

    iFrame.setAttribute("src", mod + ".html");
});



let mod2 = document.getElementById("mod2Select");
mod2.addEventListener("change", () => {
    
    const mod2 = document.getElementById("mod2Select").value;
    const iFrame2 = document.getElementById("iFrameMod2");

    iFrame2.setAttribute("src", mod2 + ".html");
});



let mod3 = document.getElementById("mod3Select");
mod3.addEventListener("change", () => {
    
    const mod3 = document.getElementById("mod3Select").value;
    const iFrame3 = document.getElementById("iFrameMod3");

    iFrame3.setAttribute("src", mod3 + ".html");
});



    let colorTrigger = document.getElementById("primaryColorSelector");

    colorTrigger.addEventListener("change", () => {

        const color = document.getElementById("primaryColorSelector").value;

        const style = document.createElement("style");
        const body = document.getElementById("body");

        body.appendChild(style);

        style.append("body {background-color: " + color + ";}");
    })