setTimeout(SetHref, 1000);

//Write a function that allows us to link back to our Blog from the custom image
function SetHref() {
    document
        .querySelector(".topbar-wrapper a")
        .setAttribute("href", "/Home/Index");    
}