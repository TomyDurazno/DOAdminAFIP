using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DOAdminAFIP.Extensions;
using NPOI.HSSF.UserModel;

namespace DOAdminAFIP.Core
{
    public class AFIPParser : IRawInputParser
    {
        #region ToAFIPModel

        List<AFIPRawModel> ToAFIPModel(string result)
        {
            AFIPRawModel InputToAFIPModel(string input)
            {
                var model = new AFIPRawModel();

                string acum;

                (model.Año, acum) = Utils.Split(input, 4);

                (model.Mes, acum) = Utils.Split(acum, 2);

                (model.Dia, acum) = Utils.Split(acum, 2);

                (model.TipoDeComprobante, acum) = Utils.Split(acum, 3);

                (model.PuntoDeVenta, acum) = Utils.Split(acum, 5);

                (model.NumeroComprobanteDesde, acum) = Utils.Split(acum, 20);

                (model.NumeroComprobanteHasta, acum) = Utils.Split(acum, 20);

                (model.CodigoDocumento, acum) = Utils.Split(acum, 2);

                (model.DataAleatoria, acum) = Utils.Split(acum, 50);

                (model.MontoFactura, acum) = Utils.Split(acum, 15);

                model.DataAleatoria2 = acum;

                model.Validate(input);

                return model;
            }

            var models = new List<AFIPRawModel>();

            foreach (var line in result.SplitBy(Environment.NewLine))
                models.Add(InputToAFIPModel(line));

            return models;
        }

        #endregion

        #region Interface Implementation

        public WorkBookGenerationResult GenerateWorkBook(string content, string nombre_workbook)
        {
            var models = ToAFIPModel(content);

            var matrix = new List<string[]>() { AFIPDTO.Nombres };

            var dtos = models.Select(m => m.DTO);

            matrix.AddRange(dtos.Select(d => d.Rows));

            var workbook = Utils.GenerateWorkbook(matrix);

            return new WorkBookGenerationResult()
            {
                Nombre = nombre_workbook,
                Workbook = workbook,
                IsValidated = models.All(m => m.IsValidated),
                MontoFinal = dtos.Select(d => d.MontoFactura).Sum()
            };
        }

        #endregion
    }

    #region Models

    public class AFIPRawModel
    {
        #region To DTO

        public AFIPDTO DTO => ToDto();

        AFIPDTO ToDto()
        {
            var dto = new AFIPDTO();

            try
            {
                int Int(string s) => Convert.ToInt32(s);

                dto.Fecha = new DateTime(Int(Año), Int(Mes), Int(Dia));
            }
            catch { }

            if (int.TryParse(TipoDeComprobante, out int tipoComprobante))
                dto.TipoDeComprobante = tipoComprobante;

            if (int.TryParse(PuntoDeVenta, out int puntoDeVenta))
                dto.PuntoDeVenta = puntoDeVenta;

            if (int.TryParse(NumeroComprobanteDesde, out int numeroComprobanteDesde))
                dto.NumeroComprobanteDesde = numeroComprobanteDesde;

            if (int.TryParse(NumeroComprobanteHasta, out int numeroComprobanteHasta))
                dto.NumeroComprobanteHasta = numeroComprobanteHasta;

            if (int.TryParse(CodigoDocumento, out int codigoDocumento))
                dto.CodigoDocumento = codigoDocumento;

            dto.DataAleatoria = DataAleatoria;

            var size = MontoFactura.Count();

            (string monto, string monedas) = Utils.Split(MontoFactura, size - 2);

            var style = NumberStyles.AllowDecimalPoint;
            var provider = new CultureInfo("en-US");

            if (decimal.TryParse($"{monto}.{monedas}", style, provider, out decimal montoFactura))
                dto.MontoFactura = montoFactura;

            dto.DataAleatoria2 = DataAleatoria2;

            return dto;
        }

        #endregion

        #region Validation

        public string InputValue() => GetType()
                                     .GetProperties()
                                     .Where(p => p.PropertyType == typeof(string))
                                     .Select(p => p.GetValue(this))
                                     .Concat();

        public bool Validate(string s) => IsValidated = s == InputValue();

        public bool IsValidated { get; private set; }

        #endregion

        #region Propiedades

        public string Año { get; set; }

        public string Mes { get; set; }

        public string Dia { get; set; }

        public string TipoDeComprobante { get; set; }

        public string PuntoDeVenta { get; set; }

        public string NumeroComprobanteDesde { get; set; }

        public string NumeroComprobanteHasta { get; set; }

        public string CodigoDocumento { get; set; }

        public string DataAleatoria { get; set; }

        public string MontoFactura { get; set; }

        public string DataAleatoria2 { get; set; }

        #endregion
    }

    public class AFIPDTO
    {
        public DateTime? Fecha { get; set; }

        public int? TipoDeComprobante { get; set; }

        public int? PuntoDeVenta { get; set; }

        public int? NumeroComprobanteDesde { get; set; }

        public int? NumeroComprobanteHasta { get; set; }

        public int? CodigoDocumento { get; set; }

        public string DataAleatoria { get; set; }

        public decimal? MontoFactura { get; set; }

        public string DataAleatoria2 { get; set; }

        public string[] Rows => new string[]
        {
        $"{Fecha?.Day}/{Fecha?.Month}/{Fecha?.Year}",
        TipoDeComprobante.ToString(),
        PuntoDeVenta.ToString(),
        NumeroComprobanteDesde.ToString(),
        NumeroComprobanteHasta.ToString(),
        CodigoDocumento.ToString(),
        DataAleatoria.ToString(),
        MontoFactura.ToString(),
        DataAleatoria2.ToString()
        };

        public static string[] Nombres => new string[]
        {
        "Fecha",
        "TipoDeComprobante",
        "PuntoDeVenta",
        "NumeroComprobanteDesde",
        "NumeroComprobanteHasta",
        "CodigoDocumento",
        "DataAleatoria",
        "MontoFactura",
        "DataAleatoria2"
        };
    }

    public class WorkBookGenerationResult
    {
        public string Nombre { get; set; }
        public HSSFWorkbook Workbook { get; set; }
        public bool IsValidated { get; set; }
        public decimal? MontoFinal { get; set; }
    }

    #endregion
}
