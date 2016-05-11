angular.module('app.routes', [])

.config(function($stateProvider, $urlRouterProvider) {

  // Ionic uses AngularUI Router which uses the concept of states
  // Learn more here: https://github.com/angular-ui/ui-router
  // Set up the various states which the app can be in.
  // Each state's controller can be found in controllers.js
  $stateProvider
    
  

  .state('menu.myFood', {
    url: '/page1',
    views: {
      'side-menu21': {
        templateUrl: 'templates/myFood.html',
        controller: 'myFoodCtrl'
      }
    }
  })
  .state('menu.ajustes', {
    url: '/page10',
    views: {
      'side-menu21': {
        templateUrl: 'templates/ajustes.html',
        controller: 'ajustesCtrl'
      }
    }
  })

  .state('cart', {
    url: '/page2',
    templateUrl: 'templates/cart.html',
    controller: 'cartCtrl'
  })

  .state('menu.cloud', {
    url: '/page3',
    views: {
      'side-menu21': {
        templateUrl: 'templates/cloud.html',
        controller: 'cloudCtrl'
      }
    }
  })

  .state('menu', {
    url: '/side-menu21',
    templateUrl: 'templates/menu.html',
    abstract:true
  })

  .state('menu.configuraO', {
    url: '/page4',
    views: {
      'side-menu21': {
        templateUrl: 'templates/configuraO.html',
        controller: 'configuraOCtrl'
      }
    }
  })

  .state('menu.receitas', {
    url: '/page5',
    views: {
      'side-menu21': {
        templateUrl: 'templates/receitas.html',
        controller: 'receitasCtrl'
      }
    }
  })

  .state('menu.receita', {
    url: '/page6',
    views: {
      'side-menu21': {
        templateUrl: 'templates/receita.html',
        controller: 'receitaCtrl'
      }
    }
  })

  .state('menu.listas', {
    url: '/page7',
    views: {
      'side-menu21': {
        templateUrl: 'templates/listas.html',
        controller: 'listasCtrl'
      }
    }
  })

  .state('menu.lista', {
    url: '/page8',
    views: {
      'side-menu21': {
        templateUrl: 'templates/lista.html'
      }
    }
  })

  .state('menu.produto', {
    url: '/page9',
    views: {
      'side-menu21': {
        templateUrl: 'templates/produto.html',
        controller: 'produtoCtrl'
      }
    }
  })

$urlRouterProvider.otherwise('/side-menu21/page1')

  

});