angular.module('app.controllers', ['ionic'])
.controller('ajustesCtrl', function($scope, $ionicModal) {
  console.log("asddas");
})  
.controller('myFoodCtrl', function($scope, $ionicModal, $rootScope) {
  console.log("asddas");

  $scope.listas = [{
    id: 0,
    name: 'Lista mensal (01/02)'
  }, {
    id: 1,
    name: 'Lista semanal (15/03)'
  }];
    $scope.receitas = [{
    id: 0,
    name: 'Receita lista mensal (01/02)'
  }, {
    id: 1,
    name: 'Receita lista semanal (15/03)'
  }];
  $ionicModal.fromTemplateUrl('templates/loginFacebook.html', {
    scope: $rootScope,
    animation: 'slide-in-up'
  }).then(function(modal) {
    $rootScope.modalfacebook = modal;
  });
  
  $rootScope.openModalFacebook = function() {
    $rootScope.modalfacebook.show();
  };

  $rootScope.closeModalFacebook = function() {
    $rootScope.modalfacebook.hide();
  };

  //Cleanup the modal when we're done with it!
  $rootScope.$on('$destroy', function() {
    $rootScope.modalfacebook.remove();
  });
  // Execute action on hide modal
  $rootScope.$on('modalfacebook.hidden', function() {
    // Execute action
  });
  // Execute action on remove modal
  $rootScope.$on('modalfacebook.removed', function() {
    // Execute action
  });

  setTimeout(function(){

    console.log(window.localStorage["1"] +  " localStorage ");
    // Verificar se ja esta logado
    if(window.localStorage["1"] != "1")
      $rootScope.openModalFacebook();
  },500);

  // Como Chamar
  $ionicModal.fromTemplateUrl('templates/comoChamar.html', {
    scope: $rootScope,
    animation: 'slide-in-up'
  }).then(function(modal) {
    $rootScope.modalComoChamar = modal;
  });
  $rootScope.openModalComoChamar = function() {
    $rootScope.modalComoChamar.show();
  };
  $rootScope.closeModalComoChamar = function() {
    $rootScope.modalComoChamar.hide();
  }; 

  // 
  $ionicModal.fromTemplateUrl('templates/boasVindas.html', {
    scope: $rootScope,
    animation: 'slide-in-up'
  }).then(function(modal) {
    $rootScope.modalBoasVindas = modal;
  });
  $rootScope.openModalBoasVindas = function() {
    $rootScope.modalBoasVindas.show();
  };
  $rootScope.closeModalBoasVindas = function() {
    $rootScope.modalBoasVindas.hide();
  };  
  
})
.controller('loginFacebookCtrl', function($scope, $ionicModal, $rootScope) {
  console.log("loginFacebookCtrl"); 
  $scope.loginFacebook = function(){
    console.log("NEXT FACEBOOK");
    $rootScope.closeModalFacebook();
    $rootScope.openModalComoChamar();
  };
}) 
.controller('comoChamarCtrl', function($scope, $ionicModal, $rootScope) {
  console.log("comoChamarCtrl");
  $scope.mudarNome = function(){
    console.log("mudou nome");
    $rootScope.closeModalComoChamar();
    $rootScope.openModalBoasVindas();
  };
})
.controller('boasVindasCtrl', function($scope, $ionicModal,$state, $rootScope) {
  $scope.config = function(){
    console.log("mudou nome");
    $rootScope.closeModalBoasVindas();
    $state.go("menu.ajustes");
  };
})
.controller('ajustesCtrl',function($scope,$ionicSwipeCardDelegate, $state, $ionicPopup) {
  top.cont = 0;
  console.log("ajustesCtrl");
  var cardTypes = [{
    title: 'Você gosta de carnes?',
    image: 'img/1.png'
  }, {
    title: 'Costuma praticar exercicios?',
    image: 'img/9.png'
  }, {
    title: 'Geralmente você consome hamburgueres?',
    image: 'img/4.png'
  }, {
    title: 'Gosta de ficar em casa?',
    image: 'img/6.png'
  }, {
    title: 'Gosta dessa especialidade italiana?',
    image: 'img/5.png'
  }, {
    title: 'Seu dia é bem corrido?',
    image: 'img/7.png'
  }, {
    title: 'O que você acha desse acompanhamento?',
    image: 'img/3.png'
  }, {
    title: 'Aprova esse prato?',
    image: 'img/2.png'
  }];

  $scope.cards = Array.prototype.slice.call(cardTypes, 0, 0);

  setTimeout(function() {
    console.log("setTimeout");
    $scope.cardSwiped();
  },500);

  $scope.cardSwiped = function(index) {
    console.log("swiped");
    $scope.addCard();
  };

  $scope.cardDestroyed = function(index) {
    $scope.cards.splice(index, 1);
  };

  $scope.addCard = function() {
    if(top.cont > 4){
      $ionicPopup.alert({
        title: 'Pronto',
        template: ''
      }).then(function(res) {
        window.localStorage["1"] = "1";
        console.log(window.localStorage["1"] +  " localStorage ");
        $state.go("menu.myFood");
      });
    }else{
      console.log(" >>> " + top.cont);
      var newCard = cardTypes[top.cont];
      top.cont++;
      newCard.id = Math.random();
      $scope.cards.push(angular.extend({}, newCard));
    }
  }
})
.controller('CardCtrl', function($scope, $ionicSwipeCardDelegate) {
  $scope.goAway = function() {
    var card = $ionicSwipeCardDelegate.getSwipeableCard($scope);
    card.swipe();
  };
})
.controller('cartCtrl', function($scope) {

})
   
.controller('cloudCtrl', function($scope) {

})
      
.controller('configuraOCtrl', function($scope, $state) {
  console.log("configuraOCtrl");
  $scope.criarLista = function(){
    console.log("criar Lista", $scope.days);
    $state.go("menu.lista");
  }
})
   
.controller('receitasCtrl', function($scope,$ionicLoading,$http) {
  console.log('receitasCtrl');
  $scope.receitas = [];
  
  // $ionicLoading.show();
    
  //   $http.get("http://hackathon.epadoca.com/Lista/Receitas").then(function(response) {
  //     $scope.receitas = response.data;
      
  //     $ionicLoading.hide();
  //   }, function (error) {
  //     console.log(error);
  //     alert("Ocorreu um erro ao buscar as informações, tente novamente");
  //     $ionicLoading.hide();
  //   });
})
   
.controller('receitaCtrl', function($scope, $ionicLoading) {  
  console.log("receitaCtrlreceitaCtrlreceitaCtrlreceitaCtrl");
  $scope.receitas = [{
    id: 0,
    name: '2 xícaras de açúcar<br />3 xícaras de farinha de trigo<br />4 colheres de margarina<br />3 ovos<br />1 e 1/2 xícara de leite de vaca<br />1 colher (sopa) bem cheia de fermento em pó'
  }, {
    id: 1,
    name: 'Receita lista semanal (15/03)'
  }];
  
  // $scope.buscarReceita = function (codigo) {
  //   //$ionicLoading.show();
    
  //   $http.get("http://hackathon.epadoca.com/Lista/Receita?codigo=" + codigo).then(function(response) {
  //     $scope.receita = response.data;
  //     //$ionicLoading.hide();
  //   }, function (error) {
  //     console.log(error);
  //     alert("Ocorreu um erro ao buscar as informações, tente novamente");
  //     //$ionicLoading.hide();
  //   });
  // }
})

.controller('listasCtrl', function($scope, $ionicLoading) {
  console.log('listasCtrl');
  $scope.listas = [{
    id: 0,
    name: 'Lista mensal (01/02)'
  }, {
    id: 1,
    name: 'Lista semanal (15/03)'
  }];
  //$ionicLoading.show();
  //$http.get("http://hackathon.epadoca.com/Lista/Listas").then(function(response) {
    //$scope.listas = response.data;
    //$ionicLoading.hide();
  //});
})
   
.controller('listaCtrl', function($scope, $cordovaBarcodeScanner,$ionicLoading) {
  console.log('listaCtrl');
  
  $scope.finalizar = function () {
	  $state.go('menu.receitas');
  }
  
  $scope.lista =  [{
    id: 0,
    flag: true,
    name: 'Feijão (2kg)'
  }, {
    id: 1,
    flag: true,
    name: 'Arroz (5kg)'
  }, {
    id: 2,
    flag: true,
    name: 'Batata (4kg)',
  }, {
    id: 3,
    flag: false,
    name: 'Mandioca (2kg)'
  }, {
    id: 4,
    flag: false,
    name: 'Laranja (1,5kg)'
  }];

  $scope.scanBarcode = function() {
        $cordovaBarcodeScanner.scan().then(function(imageData) {
            alert("TAFF MAN: " + imageData.text);
            console.log("Barcode Format -> " + imageData.format);
            console.log("Cancelled -> " + imageData.cancelled);
          //   $ionicLoading.show();
          // $http.get("http://hackathon.epadoca.com/Lista/Comprei?codigoLista=" + $scope.lista.codigo + "&codigoBarra=" + imageData.text).then(function(response) {
          //     $scope.listas = response.data;
          //     $ionicLoading.hide();
          //   });
            
        }, function(error) {
            console.log("An error happened -> " + error);
            
        });
    };
    
    $scope.finalizarLista = function () {
      $ionicLoading.show();
  
      $http.get("http://hackathon.epadoca.com/Lista/FinalizarCompra?codigoLista=" + $scope.lista.codigo).then(function(response) {
          
          $ionicLoading.hide();
        });
    }
    
    $scope.naoTenhoProduto = function(codigo) {
      $ionicLoading.show();
  
      $http.get("http://hackathon.epadoca.com/Lista<br />aoTenhoProduto?codigoLista=" + $scope.lista.codigo + "&codigo=" + codigo).then(function(response) {
          
          $ionicLoading.hide();
        });
    }

})
   
.controller('produtoCtrl', function($scope) {

})
 