using System.Collections.Generic;

namespace Hackathon.Models
{
    public class Header
    {
        public int ReturnCode { get; set; }
        public int NumberOfHits { get; set; }
        public string ResponderGLN { get; set; }
    }

    public class ResponseItem
    {
        public string CpfCnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string Gtin { get; set; }
        public string NrPrefixo { get; set; }
        public string DescTipoGtin { get; set; }
        public string MarcaProduto { get; set; }
        public string CodigoInterno { get; set; }
        public string Descricao { get; set; }
        public string DescricaoImpressao { get; set; }
        public string DtInclusao { get; set; }
        public object DtSuspensao { get; set; }
        public object DtReativacao { get; set; }
        public object DtCancelamento { get; set; }
        public object DtReutilizacao { get; set; }
        public string Pais { get; set; }
        public string NomeTipoProduto { get; set; }
        public string Lingua { get; set; }
        public string Estado { get; set; }
        public string TIPI { get; set; }
        public string CodigoRegistro { get; set; }
        public string NomeAgencia { get; set; }
        public string Largura { get; set; }
        public string Profundidade { get; set; }
        public string Altura { get; set; }
        public string PesoLiquido { get; set; }
        public string PesoBruto { get; set; }
        public string UrlMobilecom { get; set; }
        public object UrlFoto1 { get; set; }
        public object UrlFoto2 { get; set; }
        public object UrlFoto3 { get; set; }
        public string CompartilhaDado { get; set; }
        public object GtinOrigem { get; set; }
        public string Status { get; set; }
        public string DescStatus { get; set; }
        public object Observacoes { get; set; }
        public string NCM { get; set; }
        public object GtinInferior { get; set; }
        public string QtdeItens { get; set; }
        public object DescricaoInferior { get; set; }
        public string CodeSegment { get; set; }
        public string CodeFamily { get; set; }
        public string CodeClass { get; set; }
        public string CodeBrick { get; set; }
        public string CodeType { get; set; }
        public string CodeValue { get; set; }
        public string StatusPrefixo { get; set; }
        public string Imagem1 { get; set; }
        public string Imagem2 { get; set; }
        public string Imagem3 { get; set; }
        public object Contato { get; set; }
        public object InformationProviderGLN { get; set; }
        public object LastChangeDateTime { get; set; }
    }

    public class ResponseDTO
    {
        public Header Header { get; set; }
        public List<ResponseItem> ResponseItems { get; set; }
    }

    public class RootObject
    {
        public ResponseDTO ResponseDTO { get; set; }
        public bool Success { get; set; }
        public List<object> Errors { get; set; }
    }
}