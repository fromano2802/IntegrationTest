//Bootstrap the app
angular
.module("IntegrationTest", ["ngMaterial"])
.controller("MainController",["$scope", function ($scope){
	
	var vm = $scope;
	vm.test = "Hello, World!";

}]);