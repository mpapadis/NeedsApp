var //siteUrl = window.position.origin + '/',
    siteUrl ='http://localhost:2138/',
    map,
    infoWindow,
    mapOptions,
    areasArray = {};

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

$(document).ready(function () {
    $('#loginForm').on('submit', function(e) {
        if($('#username').val() == 'admin' && $('#pwd').val() == '1234') {
            e.preventDefault();
            window.position = siteUrl + 'admin.html';
        } else {
            e.preventDefault();
        }
    });

    mapOptions = {
        zoom: 15,
        center: new google.maps.LatLng(37.973105, 23.742327),
        mapTypeId: google.maps.MapTypeId.TERRAIN
    };

    areasArray = {
        "1" : {
            "id" : 1,
            "title" : "Εθνικός Κήπος",
            "position" : {lat: 37.972564, lng: 23.737436},
            "sensors" : 8,
            "status" : "open",
            "accessible" : "true",
            "humidity" : "250",
            "temperature" : "27",
            "ph" : "6"
        },
        "2" : {
            "id" : 2,
            "title" : "Βυζαντινό & Χριστιανικό Μουσείο",
            "position" : {lat: 37.974598, lng: 23.745173},
            "sensors" : 2,
            "status" : "closed",
            "accessible" : "false",
            "humidity" : "500",
            "temperature" : "20",
            "ph" : "3"
        }
    };
});

function generateMap() {
    map = new google.maps.Map($('#map-canvas')[0], mapOptions);
    for (var area in areasArray) {
        var statusColor;

        if(areasArray[area].status == "open" && areasArray[area].accessible == "true") {
            statusColor = "#26A65B";
        } else {
            statusColor = '#FF0000';
        }

        var areaCircle = new google.maps.Circle({
            strokeColor: statusColor,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: statusColor,
            fillOpacity: 0.35,
            map: map,
            center: areasArray[area].position,
            radius: Math.sqrt(areasArray[area].sensors) * 100,
            areaId: areasArray[area].id
        });

        areaCircle.addListener('click', showInfoWindow);
        infoWindow = new google.maps.InfoWindow;
    }
}

function generateAreaMap(areaId) {
    
}

function showInfoWindow(event) {
    var areaInfo = areasArray[$(this)[0].areaId];
    var contentString = '<span class="infowindowtxt"><b><a href="'+ siteUrl +'status.html?area='+ areaInfo.id +'">'+ areaInfo.title +'</a></b><br>' +
        '<b>Κατάσταση:</b> ' + areaInfo.status + '<br/>' +
        '<b>Προσβάσιμο</b>: ' + areaInfo.accessible + '</span>';

    // Replace the info window's content and position.
    infoWindow.setContent(contentString);
    infoWindow.setPosition(event.latLng);

    infoWindow.open(map);
}

function LoadWeatherApi() {
    var tempC;
    var iconUrl = '';
    var pathArray;

    var weatherSnow = ['chanceflurries', 'chancesnow', 'flurries', 'nt_chanceflurries', 'nt_chancesnow', 'nt_flurries', 'nt_snow', 'snow'];
    var weatherRain = ['chancerain', 'chancesleet', 'nt_chancerain', 'nt_chancesleet', 'nt_rain', 'nt_sleet', 'rain', 'sleet'];
    var weatherStorm = ['chancetstorms', 'nt_chancetstorms', 'nt_tstorms', 'tstorms'];
    var weatherSun = ['clear', 'sunny'];
    var weatherClouds = ['cloudy', 'fog', 'hazy', 'nt_cloudy', 'nt_fog', 'nt_hazy', 'nt_mostlycloudy', 'nt_mostlysunny', 'nt_partlycloudy', 'nt_partlysunny'];
    var weatherSunClouds = ['mostlycloudy', 'mostlysunny', 'partlycloudy', 'partlysunny'];

    $.ajax({
        type: "GET",
        url: 'http://api.wunderground.com/api/21c124355149bd2e/geolookup/conditions/q/zmw:00000.10.16716.json',
        success: function (response) {
            tempC = response.current_observation.temp_c;
            iconUrl = response.current_observation.icon_url;

            pathArray = iconUrl.split('/');
            var imgNameArr = pathArray[6].split('.');
            var imgName = imgNameArr[0];
    
            $('.tempC').text(tempC);
            $('.humidityC').text(response.current_observation.relative_humidity);
            $('.windDegr').text(response.current_observation.wind_degrees);
            $('.windKph').text(response.current_observation.wind_kph);

            if($.inArray(imgName, weatherSnow) != -1) {
                $('.weather .icon').addClass('flurries');
                $('.icon.flurries').html('<div class="cloud"></div><div class="snow"><div class="flake"></div><div class="flake"></div></div>');
            } else if($.inArray(imgName, weatherRain) != -1) {
                $('.weather .icon').addClass('rainy');
                $('.icon.rainy').html('<div class="cloud"></div><div class="rain"></div>');
            } else if($.inArray(imgName, weatherStorm) != -1) {
                $('.weather .icon').addClass('thunder-storm');
                $('.icon.thunder-storm').html('<div class="cloud"></div><div class="lightning"><div class="bolt"></div><div class="bolt"></div></div>');
            } else if($.inArray(imgName, weatherSun) != -1) {
                $('.weather .icon').addClass('sunny');
                $('.icon.sunny').html('<div class="sun"><div class="rays"></div></div>');
            } else if($.inArray(imgName, weatherClouds) != -1) {
                $('.weather .icon').addClass('cloudy');
                $('.icon.cloudy').html('<div class="cloud"></div><div class="cloud"></div>');
            } else if($.inArray(imgName, weatherSunClouds) != -1) {
                $('.weather .icon').addClass('sun-shower');
                $('.icon.sun-shower').html('<div class="cloud"></div><div class="sun"><div class="rays"></div></div>');
            }
        }
    });

    $.ajax({
        type: "GET",
        url: 'http://api.wunderground.com/api/21c124355149bd2e/geolookup/forecast/q/zmw:00000.10.16716.json',
        success: function (response) {
            $('.tempC_1').text(response.forecast.simpleforecast.forecastday[1].low.celsius + ' - ' + response.forecast.simpleforecast.forecastday[1].high.celsius);
            $('.humidityC_1').text(response.forecast.simpleforecast.forecastday[1].avehumidity + ' %');
            $('.windKph_1').text(response.forecast.simpleforecast.forecastday[1].avewind.kph + ' χλμ');

            $('.title_2').text(response.forecast.simpleforecast.forecastday[2].date.day + '/' + response.forecast.simpleforecast.forecastday[2].date.month + '/' + response.forecast.simpleforecast.forecastday[2].date.year);
            $('.tempC_2').text(response.forecast.simpleforecast.forecastday[2].low.celsius + ' - ' + response.forecast.simpleforecast.forecastday[2].high.celsius);
            $('.humidityC_2').text(response.forecast.simpleforecast.forecastday[2].avehumidity + ' %');
            $('.windKph_2').text(response.forecast.simpleforecast.forecastday[2].avewind.kph + ' χλμ');

            $('.title_3').text(response.forecast.simpleforecast.forecastday[3].date.day + '/' + response.forecast.simpleforecast.forecastday[3].date.month + '/' + response.forecast.simpleforecast.forecastday[3].date.year);
            $('.tempC_3').text(response.forecast.simpleforecast.forecastday[3].low.celsius + ' - ' + response.forecast.simpleforecast.forecastday[3].high.celsius);
            $('.humidityC_3').text(response.forecast.simpleforecast.forecastday[3].avehumidity + ' %');
            $('.windKph_3').text(response.forecast.simpleforecast.forecastday[3].avewind.kph + ' χλμ');
        }//
    });
}

function startRefresh() {
    setTimeout(startRefresh, 60000);
    $.get('http://192.168.0.82/api/ArduinoStations', function(data) {
        $('#checkrefresh').html(data);    
    });
}

function drawChart() {
    // Create the data table.
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Topping');
    data.addColumn('number', 'Slices');
    data.addRows([
        ['Mushrooms', 3],
        ['Onions', 1],
        ['Olives', 1],
        ['Zucchini', 1],
        ['Pepperoni', 2]
    ]);

    // Set chart options
    var options = {'title':'How Much Pizza I Ate Last Night',
        'width':400,
        'height':300
    };

    // Instantiate and draw our chart, passing in some options.
    var chart = new google.visualization.PieChart(document.getElementById('chart_div'));
    chart.draw(data, options);
}