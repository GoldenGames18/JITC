var title = document.title;

document.getElementsByClassName("active").item(0).classList.remove("active");

if (title.includes("Dashboard")) {
    document.getElementById("dashboard").classList.add("active");
} else
if (title.includes("Utilisateurs")) {
    document.getElementById("user").classList.add("active");
}else
if (title.includes("Aéroport")) {
    document.getElementById("airport").classList.add("active");
}else
if (title.includes("Trajet")) {

    document.getElementById("route").classList.add("active");
}else
if (title.includes("Garage")) {
    document.getElementById("garage").classList.add("active");
}else
if (title.includes("Helicoptere")) {
    document.getElementById("helicoptere").classList.add("active");
}else
if (title.includes("Statistique")) {
    document.getElementById("statistique").classList.add("active");
}else
if (title.includes("Rapport")) {

    document.getElementById("report").classList.add("active");
}






