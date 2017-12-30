var //siteUrl = window.position.origin + '/',
    siteUrl ='http://localhost:2138/',
    map,
    infoWindow,
    mapOptions,
    areasArray = [];

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

    $.ajax({
        type: "get",
        url: "http://10.16.248.133/api/ArduinoStations/",
        success: function (response) {
            response.forEach(function(element) {
                var latlng = element.location.split(',');
                areasArray.push(
                    {
                        "id" : element.id,
                        "title" : element.description,
                        "position" : {lat: parseFloat(latlng[0]), lng: parseFloat(latlng[1])},
                        "status" : element.waterStatus,
                        "uri" : element.uri
                    }
                );
            }, this);
        }
    });
});

function generateMap() {
    var bounds = new google.maps.LatLngBounds();
    
    mapOptions = {
        zoom: 16,
        center: new google.maps.LatLng(37.992073, 23.707646),
        mapTypeId: google.maps.MapTypeId.TERRAIN
    };
    
    setTimeout(function() {
        areasArray.forEach(function(area) {
            var statusColor;
            
            //cLatLng = new google.maps.LatLng(parseFloat(area.position.lat), parseFloat(area.position.lng.replace(/\s+/g, '')));
            //bounds.extend(cLatLng);

            if(area.status == true/* && area.accessible == "true"*/) {
                statusColor = "#26A65B";
            } else {
                statusColor = '#FF0000';
            }

            //console.log(initLatLng);
            var areaCircle = new google.maps.Circle({
                strokeColor: statusColor,
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: statusColor,
                fillOpacity: 0.35,
                map: map,
                center: area.position,
                radius: 100,
                areaId: area.id
            });

            areaCircle.addListener('click', showInfoWindow);
            infoWindow = new google.maps.InfoWindow;
        }, this);
    }, 2000);
    
    map = new google.maps.Map($('#map-canvas')[0], mapOptions);
    //map.fitBounds(bounds);
}

function showInfoWindow(event) {
    var areaInfo;
    for (var i in areasArray) {
        if (areasArray[i].id == $(this)[0].areaId) {
            areaInfo = areasArray[i];
        }
    }
    
    var contentString = '<span class="infowindowtxt"><b><a href="'+ siteUrl +'status.html?area='+ areaInfo.id +'">'+ areaInfo.title +'</a></b><br>' +
        '<b>Κατάσταση:</b> ' + areaInfo.status + '<br/>'/* +
        '<b>Προσβάσιμο</b>: ' + areaInfo.accessible + '</span>'*/;

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

/*function startRefresh() {
    setTimeout(startRefresh, 60000);
    $.get('http://10.16.248.133/api/ArduinoStations', function(data) {
        $('#checkrefresh').html(data);    
    });
}*/

function drawChart() {
    var data = google.visualization.arrayToDataTable([
        ['Μήνας', 'Happy Trees', 'Unhappy Trees'],
        ['Ιανουάριος', 460, 600],
        ['Φεβρουάριος', 300, 460],
        ['Μάρτιος', 660, 1120],
        ['Απρίλιος', 300, 540],
        ['Μάιος', 420, 540],
        ['Ιούνιος', 420, 540],
        ['Ιούλιος', 420, 540],
        ['Αύγουστος', 470, 540],
        ['Σεπτέμβρης', 470, 540],
        ['Οκτώβρης', 470, 540],
        ['Νοέμβρης', 470, 540],
        ['Δεκέμβρης', 470, 540]
    ]);

    var options = {
        title: 'Κατανάλωση νερού σε κυβικά',
        curveType: 'function',
        legend: { position: 'bottom' }
    };

    var chart = new google.visualization.LineChart(document.getElementById('curve_chart'));

    chart.draw(data, options);
}

function drawAxisTickColors() {
    var data = new google.visualization.DataTable();
    data.addColumn('year', 'Έτος');
    data.addColumn('number', 'Happy Trees');
    data.addColumn('number', 'Unhappy Trees');

    data.addRows([
        [{v: [8, 0, 0], f: '2016'}, 1, .25],
        [{v: [9, 0, 0], f: '2017'}, 2, .5],
    ]);

    var options = {
        title: 'Συγκριτικό κόστος σύμφωνα με την τιμολογιακή πολιτική της ΔΕΥΑ του Δήμου',
        focusTarget: 'category',
        hAxis: {
            title: 'Έτος',/*
            viewWindow: {
            min: [7, 30, 0],
            max: [17, 30, 0]
            },*/
            textStyle: {
                fontSize: 14,
                color: '#053061',
                bold: true,
                italic: false
            },
            titleTextStyle: {
                fontSize: 18,
                color: '#053061',
                bold: true,
                italic: false
            }
        },
        vAxis: {
            title: 'Κόστος (σε ευρώ)',
            textStyle: {
                fontSize: 18,
                color: '#67001f',
                bold: false,
                italic: false
            },
            titleTextStyle: {
                fontSize: 18,
                color: '#67001f',
                bold: true,
                italic: false
            }
        }
    };

    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
    chart.draw(data, options);
}