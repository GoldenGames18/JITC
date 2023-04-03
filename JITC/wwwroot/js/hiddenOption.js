function detectChangeStart(selectAirport) {

    var selectobject = document.getElementById("end");
    for (var i = 0; i < selectobject.length; i++) {
        
            selectobject[i].hidden = false;
    }


    for (var i = 0; i < selectobject.length; i++) {

        if (selectobject.options[i].value === selectAirport.value && selectobject.options[i].value != "----------------")
            selectobject[i].hidden = true;
    }
}

function detectChangeEnd(selectAirport) {
    var selectobject = document.getElementById("start");
    for (var i = 0; i < selectobject.length; i++) {
       
            selectobject[i].hidden = false;
    }
    for (var i = 0; i < selectobject.length; i++) {
        if (selectobject.options[i].value === selectAirport.value && selectobject.options[i].value != "----------------")
            selectobject[i].hidden = true;
    }
}


function helicopSize(selectHelico) {

    var heli = document.getElementById("heli")

    var listOfHelicopter = document.getElementById("helicop")

    for (var i = 0; i < listOfHelicopter.length; i++) {

        if (listOfHelicopter.options[i].value === selectHelico.value) {
            document.getElementById("size").value = "1";
            document.getElementById("size").max =  listOfHelicopter[i].getAttribute('data');
        }
    }




}