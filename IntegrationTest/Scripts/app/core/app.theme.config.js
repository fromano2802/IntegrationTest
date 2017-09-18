
angular
    .module("IntegrationTest")
    .config([
        "$mdThemingProvider", function($mdThemingProvider) {

            $mdThemingProvider.theme("default")
                .primaryPalette("blue");

        }
    ]);