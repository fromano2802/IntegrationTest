angular
    .module("IntegrationTest")
    .config([
        "$routeProvider", function($routeProvider) {
            debugger;
            $routeProvider
                .otherwise("",
                    {
                        templateUrl: "js/app/home/home.template.html"
                    });

        }
    ]);