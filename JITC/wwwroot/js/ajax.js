$("#start").autocomplete({
    source: function (request, reponse) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Route/FindAirportStart',
            data: { startAirport: request.term },
            success: function (data) { reponse(data) }
        })
    }
})

$("#end").autocomplete({
    source: function (request, reponse) {
        $.ajax({
            headers: { RequestVerificationToken: $("#RequestCsrfToken").val() },
            datatype: 'json',
            url: 'Route/FindAirportEnd',
            data: { startAirport: request.term },
            success: function (data) { reponse(data) }
        })
    }
})
