using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using Hackathon.Models;
using Newtonsoft.Json;

namespace Hackathon.Controllers
{
    public class ListaController : Controller
    {
        // GET api/values
        public JsonResult Criar(string alimentos, 
            bool almoco, bool janta, 
            int quantidadeDias, ObjetivoEnum objetivo, 
            string nome)
        {
            int quantidadePorDia = almoco && janta ? 2 : 1;

            IList<string> s = alimentos.Split(',');

            IList<Receita> receitas = this.GetReceitasCadastradas();

            List<IngredienteView> receitasParaProduto = new List<IngredienteView>();

            foreach (Receita receita in receitas)
            {
                if (receita.ReceitaIngredientes.Any(x => s.Any(y => y == x.Ingrediente.Nome) || s.Any(y => x.Ingrediente.KeyWord.Any(z => z == y))))
                {
                    foreach (ReceitaIngrediente receitaIngrediente in receita.ReceitaIngredientes)
                    {
                        IngredienteView ingredienteAdicionado =
                            receitasParaProduto.FirstOrDefault(x => x.Ingrediente.Codigo == receitaIngrediente.Ingrediente.Codigo);

                        if (ingredienteAdicionado == null)
                        {
                            int cod = receitasParaProduto.Count > 0 ? receitasParaProduto.Max(x => x.Codigo) + 1 : 1;

                            receitasParaProduto.Add(new IngredienteView
                            {
                                Codigo = cod,
                                Ingrediente = receitaIngrediente.Ingrediente,
                                Quantidade = receitaIngrediente.Quantidade,
                                TipoValoracao = receitaIngrediente.TipoValoracao
                            });
                        }
                        else
                        {
                            // Deixado temporariamente para calcular a quantidade de acordo com a quantidade maxima
                            if (receitaIngrediente.Quantidade > ingredienteAdicionado.Quantidade)
                            {
                                ingredienteAdicionado.Quantidade = receitaIngrediente.Quantidade;
                            }
                        }
                    }
                }
            }

            // Deixado temporariamente para calcular a quantidade de acordo com a quantidade maxima
            foreach (var v in receitasParaProduto)
            {
                v.Quantidade = (v.Quantidade * quantidadeDias) * quantidadePorDia;

                // Se ele quer ganhar ou perder peso
                // + 20%
                // - 20%
                if (objetivo.Equals(ObjetivoEnum.Ganhar))
                {
                    v.Quantidade = v.Quantidade*1.2;
                } else if (objetivo.Equals(ObjetivoEnum.Perder))
                {
                    v.Quantidade = v.Quantidade*0.8;
                }
            }

            UsuarioView usuarioView = new UsuarioView();

            if (Session["usuario"] != null)
            {
                usuarioView = (UsuarioView)Session["usuario"];
            }



            IList<ListaView> listaViewL = usuarioView.ListaViewL;

            int codigo = listaViewL.Count > 0 ? listaViewL.Max(x => x.Codigo) + 1 : 1;

            var listaView = new ListaView
            {
                Codigo = codigo,
                Nome = nome,
                IngredienteL = receitasParaProduto
            };

            listaViewL.Add(listaView);

            Session["usuario"] = usuarioView;

            return Json(listaView, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TenhoProduto(int codigoLista, int codigo, bool tenho)
        {
            UsuarioView usuarioView = (UsuarioView) Session["usuario"];

            IList<ListaView> listaViewL = usuarioView.ListaViewL;

            ListaView listaView = listaViewL.First(x => x.Codigo == codigoLista);
            IngredienteView ingredienteView = listaView.IngredienteL.First(x => x.Codigo == codigo);

            ingredienteView.Tenho = tenho;

            Session["usuario"] = usuarioView;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Comprei(int codigoLista, string codigoBarra)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    #region Verifica autenticacao

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://inbar-producao-ws.azurewebsites.net/search");

                    try
                    {
                        requestMessage.Headers.Add("deviceid", "hackathon-team-00524");
                        requestMessage.Headers.Add("secret", "62a7970e-1a72-494d-a27a-90d15cfba392");
                        requestMessage.Headers.Add("Cache-Control", "no-cache");

                        requestMessage.Content = CreateContent(codigoBarra);
                    }
                    catch (Exception) { }

                    var responseMessage = client.SendAsync(requestMessage).Result;

                    var responseBody = responseMessage.Content.ReadAsStringAsync().Result;

                    var feedbackRequest = VerifyStatusRequest(responseMessage);

                    if (feedbackRequest.Equals("sucessed"))
                    {
                        UsuarioView usuarioView = (UsuarioView) Session["usuario"];
                        ListaView listaView = usuarioView.ListaViewL.First(x => x.Codigo == codigoLista);
                        IngredienteView ingredienteView = listaView.IngredienteL.First(x => responseBody.Contains(x.Ingrediente.Nome)); //listaView.IngredienteL.First(x => x.Codigo == codigo);


                        //string[] ob =  (string[])JsonConvert.DeserializeObject(responseBody);
                        //var obj1 = ob[0];

                        //ingredienteView.IngredienteNome = 
                        ingredienteView.CodigoBarra = codigoBarra;
                        ingredienteView.Tenho = true;
                        ingredienteView.CodigoBarraInfo = responseBody;
                        usuarioView.TotalPontos += 3;

                        Session["usuario"] = usuarioView;

                        

                        return Json(new
                        {
                            CodigoBarraInfo = responseBody,
                            TotalPontos = usuarioView.TotalPontos
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Debug.WriteLine("ERROR");
                    }
                }
                    #endregion
            }
            catch (Exception ex)
            {
                //grava no log o erro gerado.
                Debug.WriteLine(ex.Message);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinalizarCompra(int codigoLista)
        {
            UsuarioView usuarioView = (UsuarioView)Session["usuario"];
            ListaView listaView = usuarioView.ListaViewL.First(x => x.Codigo == codigoLista);
            
            usuarioView.TotalPontos += 50;
            listaView.ListaFinalizada = true;

            Session["usuario"] = usuarioView;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NaoTenhoProduto(int codigoLista, int codigo)
        {
            UsuarioView usuarioView = (UsuarioView)Session["usuario"];
            ListaView listaView = usuarioView.ListaViewL.First(x => x.Codigo == codigoLista);
            IngredienteView ingredienteView = listaView.IngredienteL.First(x => x.Codigo == codigo);
            ingredienteView.Tenho = false;

            if (!string.IsNullOrWhiteSpace(ingredienteView.CodigoBarra))
            {
                usuarioView.TotalPontos -= 3;
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Receitas()
        {
            UsuarioView usuarioView = (UsuarioView) Session["usuario"];
            IList<ListaView> listaViewL = usuarioView.ListaViewL;
            IList<Receita> receitaL = new List<Receita>();
            IList<Receita> receitaCadastradaL = this.GetReceitasCadastradas();

            foreach (ListaView listaView in listaViewL)
            {
                foreach (var ingrediente in listaView.IngredienteL)
                {
                    foreach (Receita receita in receitaCadastradaL.Where(x => x.Nome == ingrediente.Ingrediente.Nome))
                    {
                        if (!receitaL.Any(y => y.Codigo == receita.Codigo))
                        {
                            receitaL.Add(receita);
                        }
                    }
                    
                }
            }

            return Json(receitaL, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Receita(int codigo)
        {
            IList<Receita> receitaCadastradaL = this.GetReceitasCadastradas();
            Models.Receita receita = receitaCadastradaL.First(x => x.Codigo == codigo);

            return Json(receita, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Pontos()
        {
            if (Session["usuario"] == null)
                return Json(0, JsonRequestBehavior.AllowGet);

            UsuarioView usuarioView = (UsuarioView)Session["usuario"];

            return Json(usuarioView.TotalPontos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Promocao()
        {
            if (Session["usuario"] == null)
                return Json(true, JsonRequestBehavior.AllowGet);

            UsuarioView usuarioView = (UsuarioView) Session["usuario"];
            IList<Promocao> promocaoL = GetPromocao();

            foreach (ListaView listaView in usuarioView.ListaViewL.Where(x => !x.ListaFinalizada))
            {
                foreach (Promocao promocao in promocaoL)
                {
                    if (
                    listaView.IngredienteL.Any(
                        x => promocao.IngredienteL.Any(y => y.Codigo == x.Ingrediente.Codigo)))
                    {
                        return Json(promocao, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Listas()
        {
            if (Session["usuario"] == null)
                return Json(true, JsonRequestBehavior.AllowGet);

            UsuarioView usuarioView = (UsuarioView)Session["usuario"];

            return Json(usuarioView.ListaViewL, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Lista(int id)
        {
            UsuarioView usuarioView = (UsuarioView)Session["usuario"];
            ListaView listaView = usuarioView.ListaViewL.FirstOrDefault(x => x.Codigo == id);

            return Json(listaView, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        private IList<Receita> GetReceitasCadastradas()
        {
            IList<Receita> receitaL = new List<Receita>();

            if (Session["receita"] != null)
            {
                return (List<Receita>)Session["receita"];
            }

            var frango = new Ingrediente(1, "Filé de Frango à Parmegiana", new List<string> { "Ave", "Aves", "Frango" });
            var alho = new Ingrediente(1, "Alho", new List<string> { "Alho" });
            var limao = new Ingrediente(1, "Limão", new List<string> { "", "" });
            var ervas = new Ingrediente(1, "Ervas Finas", new List<string> { "", "" });
            var sal = new Ingrediente(1, "Sal", new List<string> { "", "" });
            var ovo = new Ingrediente(1, "Ovo", new List<string> { "", "" });
            var farinhaRosca = new Ingrediente(1, "Farinha de Rosca", new List<string> { "", "" });
            var cebola = new Ingrediente(1, "Cebola", new List<string> { "", "" });
            var tomate = new Ingrediente(1, "Tomate", new List<string> { "", "" });
            var tomateMolho = new Ingrediente(1, "Molho de Tomate", new List<string> { "", "" });
            var azeite = new Ingrediente(1, "Azeite", new List<string> { "", "" });
            var tableteCaldoGalinha = new Ingrediente(1, "Tablete Caldo de Galinha", new List<string> { "", "" });
            var mussarela = new Ingrediente(1, "Mussarela", new List<string> { "", "" });
            var peixe = new Ingrediente(1, "Filé de Peixe (Tilápia)", new List<string> { "Peixe", "Mar", "Tilápia" });
            var batata = new Ingrediente(1, "Batata", new List<string> { "", "" });
            var alcaparras = new Ingrediente(1, "Alcaparras", new List<string> { "", "" });
            var pimnetao = new Ingrediente(1, "Pimentão", new List<string> { "", "" });
            var cacaoPostas = new Ingrediente(1, "Cação em Postas", new List<string> { "", "" });
            var sazon = new Ingrediente(1, "Sazón Verde", new List<string> { "", "" });
            var caneloni = new Ingrediente(1, "Canelone", new List<string> { "Canelone", "Mussarela", "Presunto","Massa" });
            var presunto = new Ingrediente(1, "Presunto", new List<string> { "", "" });
            var queijoRalado = new Ingrediente(1, "Queijo Ralado", new List<string> { "Queijo", "" });
            var oleo = new Ingrediente(1, "Oleo", new List<string> { "", "" });
            //var cenoura = new Ingrediente(1, "Cenoura", new List<string> { "Vegana", "Vegetariana" });
            //var frango = new Ingrediente(1, "Pepino em", new List<string> { "Vegana", "Vegetariana" });
            //var frango = new Ingrediente(1, "Cenoura", new List<string> { "Vegana", "Vegetariana" });
            //var frango = new Ingrediente(1, "Cenoura", new List<string> { "Vegana", "Vegetariana" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });
            //var frango = new Ingrediente(1, "", new List<string> { "", "" });

            #region Filé de Frango à Parmegiana

            receitaL.Add(new Receita
            {
                Nome = "Frango Gralhado",
                Nivel = "Fácil",
                TempoPreparo = "40 MIN",
                Codigo = 1,
                ModoPreparo = @"Tempere os filés de frango com o alho, suco de limão, ervas finas e sal a gosto <br /> 
Deixe na geladeira por 30 minutos <br /> Após esse tempo, passe nos ovos batidos e na farinha de rosca <br />
Frite em óleo não muito quente <br /> Retire e deixe descansar sobre papel absorvente",                                               
                ReceitaIngredientes = new List<ReceitaIngrediente> 
                { 
                    new ReceitaIngrediente
                    {
                        Ingrediente = frango,
                        Quantidade = 0.500,
                        TipoValoracao = "Kg",
                        IngredientePreparo = frango.Nome
                    }, 
                    new ReceitaIngrediente
                    {
                        Ingrediente = alho,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Dente de alho picadinho"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = limao,
                        Quantidade = 0,
                        TipoValoracao = "Lt",
                        IngredientePreparo = "Suco de Limão a gosto"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = ervas,
                        Quantidade = 0,
                        TipoValoracao = "Gr",
                        IngredientePreparo = "Ervas finas a gosto"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = sal,
                        Quantidade = 0,
                        TipoValoracao = "Gr",
                        IngredientePreparo = "Sal a gosto"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = ovo,
                        Quantidade = 2,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Ovos batidos com 1 pitada de Sal"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = farinhaRosca,
                        Quantidade = 2,
                        TipoValoracao = "Un",
                        IngredientePreparo = "xícaras (chá) de farinha de rosca"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = cebola,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Cebola picadinha"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = tomate,
                        Quantidade = 2,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Tomates maduros picadinhos"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = tomateMolho   ,
                        Quantidade = 1,
                        TipoValoracao = "Cx",
                        IngredientePreparo = tomateMolho.Nome
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = azeite,
                        Quantidade = 3,
                        TipoValoracao = "Cl",
                        IngredientePreparo = "Colheres (sopa) de azeite"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = tableteCaldoGalinha,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Tabletinho de caldo de galinha"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = mussarela,
                        Quantidade = 250,
                        TipoValoracao = "Gr",
                        IngredientePreparo = "Mussarela em Fatias"
                    }
                }
            });

            #endregion

            #region Filé de Peixe Assado

            receitaL.Add(new Receita
            {
                Nome = "Filé de Peixe Assado",
                Nivel = "Média",
                TempoPreparo = "40 MIN",
                Codigo = 2,
                ModoPreparo = @"Tempere o filé de peixe com sal e alho e reserve <br /> 
Misture o tomate, cebola, pimentão e alcaparras e tempere com um pouco de sal e junte o cheiro verde e coentro <br />
Unte um refratário com azeite, e forre com as batatas cruas <br /> Cubra as batatas com o peixe e por cima distribua a mistura do tomate <br />
Regue com bastante azeite e leve ao forno por mais ou menos 30 a 40 minutos <br />
Quando secar o líquido que acumula no fundo da forma quando está assando e ficar dourado está pronto <br />
Sirva com arroz intergal ou branco, é uma delícia!",                                               
                ReceitaIngredientes = new List<ReceitaIngrediente> 
                { 
                    new ReceitaIngrediente
                    {
                        Ingrediente = peixe,
                        Quantidade = 0.500,
                        TipoValoracao = "Kg",
                        IngredientePreparo = peixe.Nome
                    }, 
                    new ReceitaIngrediente
                    {
                        Ingrediente = batata,
                        Quantidade = 4,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Batatas grande"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = tomate,
                        Quantidade = 2,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Tomates picadinhos"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = pimnetao,
                        Quantidade = 0.500,
                        TipoValoracao = "Un",
                        IngredientePreparo = pimnetao.Nome
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = sal,
                        Quantidade = 0,
                        TipoValoracao = "Gr",
                        IngredientePreparo = "Sal a gosto"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = cebola,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Cebola média picada em cubos"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = alcaparras,
                        Quantidade = 0.500,
                        TipoValoracao = "Gr",
                        IngredientePreparo = alcaparras.Nome
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = azeite,
                        Quantidade = 0,
                        TipoValoracao = "Gr",
                        IngredientePreparo = "Azeite a gosto"
                    }
                }
            });

            #endregion

            #region CAÇÃO AO MOLHO VERMELHO

            receitaL.Add(new Receita
            {
                Nome = "Cação ao molho vermelho",
                Nivel = "Média",
                TempoPreparo = "45 MIN",
                Codigo = 3,
                ModoPreparo = @"Em uma panela, adicione o óleo e leve ao fogo, adicione a cebola e refogue bem<br />
Adicione o molho de tomate e a pimenta<br />
Acrescente a água e acerte o sal a gosto, deixe no fogo até ferver <br/>
Pegue um refratário e unte o fundo com manteiga, despeje a metade do molho e reserve<br/>
Pegue uma fatia de massa, por cima coloque uma fatia de presunto e uma de mussarela, enrole o canelone e coloque no refratário reservado<br/>
Repita o mesmo processo até acabar a massa<br/>
Por cima, adicione o restante do molho, salpique o queijo ralado e leve para gratinar por cerca de 40 minutos em forno médio",                                               
                ReceitaIngredientes = new List<ReceitaIngrediente> 
                { 
                    new ReceitaIngrediente
                    {
                        Ingrediente = caneloni,
                        Quantidade = 1,
                        TipoValoracao = "Pc",
                        IngredientePreparo = "Massa pronta para canelone"
                    }, 
                    new ReceitaIngrediente
                    {
                        Ingrediente = tomateMolho,
                        Quantidade = 2,
                        TipoValoracao = "Lt",
                        IngredientePreparo = tomateMolho.Nome
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = mussarela,
                        Quantidade = 0.50,
                        TipoValoracao = "Kg",
                        IngredientePreparo = "Mussarela Fatiada"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = presunto,
                        Quantidade = 0.5 ,
                        TipoValoracao = "Kg",
                        IngredientePreparo = "Presunto Fatiado"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = queijoRalado,
                        Quantidade = 100,
                        TipoValoracao = "Gr",
                        IngredientePreparo = queijoRalado.Nome
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = oleo,
                        Quantidade = 0,
                        TipoValoracao = "Cl",
                        IngredientePreparo = "1 colhe (sopa) de óleo"
                    }
                }
            });

            #endregion

            #region CANELONE DE MUSSARELA COM PRESUNTO

            receitaL.Add(new Receita
            {
                Nome = "Canelone de Mussarela com Presunto",
                Nivel = "Fácil",
                TempoPreparo = "35 MIN",
                Codigo = 4,
                ModoPreparo = @"Tempere as postas com o suco de limão, 2 envelopes de sazón e 2 colheres de sal e deixe tomar gosto por 20 minutos<br />
Numa panela grande, aqueça o azeite em fogo alto e refogue o alho e a cebola por 4 minutos, ou até dourarem <br />
Junte a polpa de tomate, a água, o açúcar o sazón e o sal restantes, e cozinhe por 5 minutos <br />
Acrescente as postas de cação e deixe cozinhar por mais 15 minutos, com a panela semitampada, ou até que o peixe esteja macio",                                               
                ReceitaIngredientes = new List<ReceitaIngrediente> 
                { 
                    new ReceitaIngrediente
                    {
                        Ingrediente = cacaoPostas,
                        Quantidade = 1,
                        TipoValoracao = "Kg",
                        IngredientePreparo = peixe.Nome
                    }, 
                    new ReceitaIngrediente
                    {
                        Ingrediente = limao,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Suco de 1 limão"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = sazon,
                        Quantidade = 3,
                        TipoValoracao = "Un",
                        IngredientePreparo = "3 envelopes"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = azeite,
                        Quantidade = 0 ,
                        TipoValoracao = "Cl",
                        IngredientePreparo = "1 Colher"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = alho,
                        Quantidade = 0,
                        TipoValoracao = "Un",
                        IngredientePreparo = "2 dentes de alho espremidos"
                    },
                    new ReceitaIngrediente
                    {
                        Ingrediente = cebola,
                        Quantidade = 1,
                        TipoValoracao = "Un",
                        IngredientePreparo = "Cebola média picada"
                    }
                }
            });

            #endregion

            Session["receita"] = receitaL;

            return receitaL;
        }

        private static FormUrlEncodedContent CreateContent(string codigoBarra)
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", codigoBarra),
                new KeyValuePair<string, string>("codeType", "GTIN")
            });
        }

        private static string VerifyStatusRequest(HttpResponseMessage responseMessage)
        {
            int statusCode = (int)responseMessage.StatusCode;

            if (statusCode == 401)
                return "Autorização negada para esta requisição";
            else if (statusCode > 399 && statusCode < 418)
                return string.Format("Não foi possivel completar a requisição StatusCode {0}", statusCode);
            else
                return "sucessed";
        }

        private IList<Promocao> GetPromocao()
        {
            if (Session["promocao"] == null)
            {
                var frango = new Ingrediente(1, "Filé de Frango", new List<string> { "Ave", "Aves", "Frango" });
                var alho = new Ingrediente(1, "Alho", new List<string> { "Alho" });
                var sal = new Ingrediente(1, "Sal", new List<string> { "", "" });
                var cebola = new Ingrediente(1, "Cebola", new List<string> { "", "" });
                var tomate = new Ingrediente(1, "Tomate", new List<string> { "", "" });
                var arroz = new Ingrediente(1, "Arroz", new List<string> { "", "" });

                IList<Promocao> promocaoL = new List<Promocao>
                {
                    new Promocao
                    {
                        Nome = "Queima de Estoque 50% de desconto no Arroz",
                        IngredienteL = new List<Ingrediente>
                        {
                            frango,
                            alho, 
                            sal,
                            cebola,
                            tomate,
                            arroz
                        }
                    }
                };

                Session["promocao"] = promocaoL;
            }

            return (List<Promocao>)Session["promocao"];
        }

        #endregion
    }
}
