var siteUrl = window.location.origin + '/';

$(document).ready(function () {
    $('#loginForm').on('submit', function(e) {
        if($('#username').val() == 'admin' && $('#pwd').val() == '1234') {
            e.preventDefault();
            window.location = siteUrl + 'admin.html';
        } else {
            e.preventDefault();
        }
    });
});

function generateMap() {
    var mapOptions = {
            zoom: 15,
            center: new google.maps.LatLng(37.973105, 23.742327),
            mapTypeId: google.maps.MapTypeId.TERRAIN
        },
        map = new google.maps.Map($('#map-canvas')[0], mapOptions),
        spotsArray = {
            "1" : {
                "title" : "Εθνικος Κήπος",
                "center" : {lat: 37.972564, lng: 23.737436},
                "sensors" : 8,

            },
            "2" : {
                "title" : "Βυζαντινό & Χριστιανικό Μουσείο",
                "center" : {lat: 37.974598, lng: 23.745173},
                "sensors" : 2
            }
        };

    var markersArray = [];
    for (var spot in spotsArray) {
        var spotCircle = new google.maps.Circle({
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.35,
            map: map,
            center: spotsArray[spot].center,
            radius: Math.sqrt(spotsArray[spot].sensors) * 100
        });

        var marker = new google.maps.Marker({
            title:spotsArray[spot].title,
            map: map,
            position: spotsArray[spot].center,
            markObj: markersArray[spot]
        });
        
        spot.gmarker.infowindow = new google.maps.InfoWindow({
            content: "Πληροφορίες spot"
        });
        
        var infohtml = spot.gmarker.infowindow.content;
        
        google.maps.event.addListener(spot.gmarker, 'click', function() {
            closeAllInfoWindows();
            map.setCenter(this.getPosition());
            this.infowindow.open(map, this);
        });
    }

    /*spotsArray.forEach(function(element) {
        for each spot, create the area define lat & lng of the area covered
        var triangleCoords = [
            new google.maps.LatLng(25.774252, -80.190262),
            new google.maps.LatLng(18.466465, -66.118292),
            new google.maps.LatLng(32.321384, -64.75737),
            new google.maps.LatLng(25.774252, -80.190262)
        ];

        bermudaTriangle = new google.maps.Polygon({
            paths: triangleCoords,
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.35
        });

        bermudaTriangle.setMap(map);

        push it to the spots tmp array
        spotsArrayTmp.push(spotCoords);
    }, this);

    var overlay;
    USGSOverlay.prototype = new google.maps.OverlayView();*/
}