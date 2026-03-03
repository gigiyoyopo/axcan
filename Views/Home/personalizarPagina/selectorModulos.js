/*function selectMod1() {

    const mod = document.getElementById("modSelect1").value;
    const iFrame = document.getElementById("iFrameMod1");

    iFrame.setAttribute("src", mod);



}*/


let mod1 = document.getElementById("mod1Select");
mod1.addEventListener("change", () => {
    
    const mod = document.getElementById("mod1Select").value;
    const iFrame = document.getElementById("iFrameMod1");

    iFrame.setAttribute("src", mod);
});

let mod2 = document.getElementById("mod2Select");
mod2.addEventListener("change", () => {
    
    const mod2 = document.getElementById("mod2Select").value;
    const iFrame2 = document.getElementById("iFrameMod2");

    iFrame2.setAttribute("src", mod2);
});