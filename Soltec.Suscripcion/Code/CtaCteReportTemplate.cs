using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Soltec.Suscripcion.Model;

namespace Soltec.Suscripcion.Code
{
    public class CtaCteReportTemplate
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public Sujeto Sujeto { get; set; }
        public IList<MovCtaCte> MovCtaCte { get; set; }
        public string NombreEmpresa { get; set; }
        public string Path { get; set; }
        public  MemoryStream ListPDF()
        {

            Font font = new Font();
            font.Size = 7;
            Font fontTitulo = new Font();
            fontTitulo.Size = 9;
            Font fontD = new Font();
            fontD.Size = 7;
            Font fontDes = new Font();
            fontDes.Size = 5;
            //
            string simboloDivisa = "";
            float[] columnWidths = new float[] { 1.4F, 4, 1.3F, 1.7F, 1.5F, 1.8F, 1.8F, 1.8F };

            PdfPTable Table = new PdfPTable(columnWidths);
            Table.HeaderRows = 1;
            // Header
            PdfPCell cell = new PdfPCell(new Phrase("FECHA", font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("CONCEPTO", font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("COMPROBANTE", font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("F.VTO.", font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("DEBE" + simboloDivisa, font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("HABER" + simboloDivisa, font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("SALDO" + simboloDivisa, font));
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            Table.AddCell(cell);
            // Titulo a Vencido
            cell = new PdfPCell(new Phrase("Movimiento Cta.Cte Vencido", fontTitulo));
            cell.HorizontalAlignment = PdfPCell.ALIGN_JUSTIFIED;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthRight = 0;
            cell.Colspan = 8;
            Table.AddCell(cell);
            bool headerAVencer = false;
            foreach (var item in MovCtaCte)
            {
                BaseColor colorBack;

                int borderVenc = 0;
                if (item.Vencido)
                {
                    colorBack = new BaseColor(246, 246, 246);
                    borderVenc = 1;
                }
                else
                    colorBack = BaseColor.WHITE;
                if (item.Vencido == false & headerAVencer == false)
                {
                    headerAVencer = true;
                    // Titulo a Vencer
                    cell = new PdfPCell(new Phrase("Movimiento Cta.Cte a Vencer", fontTitulo));
                    cell.HorizontalAlignment = PdfPCell.ALIGN_JUSTIFIED;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    cell.Colspan = 8;
                    Table.AddCell(cell);
                }

                cell = new PdfPCell(new Phrase(item.FechaComprobante.ToShortDateString(), font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.Concepto, fontDes));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                Table.AddCell(cell);
                string numero = "";
                if (item.NumeroComprobante != null)
                    //numero = item.NumeroComprobante.Substring(5, 8);
                    cell = new PdfPCell(new Phrase(numero, font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                Table.AddCell(cell);
                string impd = "";
                cell = new PdfPCell(new Phrase(impd, font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.FechaVencimiento.ToShortDateString(), fontDes));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                Table.AddCell(cell);
                string debe = "";
                if (item.Debe != 0) debe = item.Debe.ToString("N");
                cell = new PdfPCell(new Phrase(debe, font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
                string haber = "";
                if (item.Haber != 0)
                    haber = item.Haber.ToString("N");
                cell = new PdfPCell(new Phrase(haber, font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
                cell = new PdfPCell(new Phrase(item.Saldo.ToString("N"), font));
                cell.BackgroundColor = colorBack;
                cell.Border = borderVenc;
                cell.BorderColor = BaseColor.WHITE;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                Table.AddCell(cell);
            }
            // Agregar Totales
            // Linea en Blanco
            decimal saldoVencido = 0;
            decimal saldoaVencer = 0;
            decimal saldo = 0;
            saldoVencido = MovCtaCte.LastOrDefault().SaldoVencido;
            saldo = MovCtaCte.LastOrDefault().Saldo;
            saldoaVencer = saldo - saldoVencido;
            // Sumas debe - haber saldo
            var totalDebe = MovCtaCte.Sum(s => s.Debe);
            var totalHaber = MovCtaCte.Sum(s => s.Haber);

            cell = new PdfPCell(new Phrase("SUMAS ", font));
            cell.Colspan = 5;
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalDebe.ToString("N"), font));
            cell.Border = 1;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 1;
            cell.BorderWidthLeft = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalHaber.ToString("N"), font));
            cell.Border = 1;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 1;
            cell.BorderWidthLeft = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(saldo.ToString("N"), font));
            cell.Border = 1;
            cell.BorderWidthRight = 1;
            cell.BorderWidthBottom = 1;
            cell.BorderWidthLeft = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            // Nueva Linea
            cell = new PdfPCell(new Phrase(" ", font));
            cell.Colspan = 8;
            cell.Border = 0;
            // Nueva Linea
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", font));
            cell.Colspan = 8;
            cell.Border = 0;
            // Nueva Linea
            Table.AddCell(cell);
            string tipoSaldo = saldoVencido > 0 ? "SALDO DEUDOR" : "SALDO ACREEDOR";
            cell = new PdfPCell(new Phrase(tipoSaldo + simboloDivisa, fontD));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("VENCIDO", fontD));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(saldoVencido.ToString("N"), fontD));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", fontD));
            cell.Colspan = 4;
            cell.Border = 0;
            Table.AddCell(cell);
            // //Nueva Linea
            tipoSaldo = saldoaVencer > 0 ? "SALDO DEUDOR " : "SALDO ACREEDOR ";
            cell = new PdfPCell(new Phrase(tipoSaldo + simboloDivisa, fontD));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            // Nueva Linea
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("A VENCER" + simboloDivisa, fontD));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(saldoaVencer.ToString("N"), fontD));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", fontD));
            cell.Colspan = 4;
            cell.Border = 0;
            // Nueva Linea
            Table.AddCell(cell);
            tipoSaldo = saldo > 0 ? "SALDO DEUDOR " : "SALDO ACREEDOR ";
            cell = new PdfPCell(new Phrase(tipoSaldo + simboloDivisa, fontD));
            cell.Border = 0;
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("TOTAL", fontD));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(saldo.ToString("N"), fontD));
            cell.Border = 1;
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(" ", fontD));
            cell.Colspan = 4;
            cell.Border = 0;
            Table.AddCell(cell);

            var doc = new Document(PageSize.A4, 10f, 10f, 135f, 100f);

            //var strFilePath = hostingEnvironment.ContentRootPath + @"\ReportsTemplate\";
            var strFilePath = this.Path + @"\ReportsTemplate\";

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.UNDERLINE, BaseColor.BLACK);
            var h1Font = FontFactory.GetFont(FontFactory.HELVETICA, 11, Font.NORMAL);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);

            try
            {
                MemoryStream stream = new MemoryStream();
                //var pdfWriter = PdfWriter.GetInstance(doc, new FileStream(strFilePath + fileName, FileMode.Create));
                var pdfWriter = PdfWriter.GetInstance(doc, stream);
                var textEvents = new ITextEvents();
                pdfWriter.PageEvent = textEvents;
                textEvents.Titulo = this.NombreEmpresa;
                textEvents.Titulo1 = "RESUMEN DE CTA. CTE." + simboloDivisa;
                textEvents.Cuenta = Sujeto;
                textEvents.FechaDesde = this.FechaDesde;
                textEvents.FechaHasta = this.FechaHasta;
                textEvents.ImagePath = strFilePath;
                doc.Open();
                doc.Add(Table);
                doc.Close();
                var file = stream.ToArray();
                var output = new MemoryStream();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return output;
                //return new FileStreamResult(output, "application/pdf") { FileDownloadName = "ResumenCtaCte.pdf" };
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
            finally
            {
                doc.Close();
            }
        }
        private ActionResult HandleErrorCondition(string message)
        {
            throw new NotImplementedException();
        }


    }
    public class ITextEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;
        public String Titulo { get; set; }
        public String Titulo1 { get; set; }
        public Sujeto Cuenta { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string ImagePath { get; set; }



        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(400, 150);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            //Crear Header de Reporte
            base.OnEndPage(writer, document);
            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontLite = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var Table = new PdfPTable(7);
            //Row 1
            PdfPCell cell = new PdfPCell(new Phrase(this.Titulo, baseFontBig));
            cell.Border = 0;
            cell.Colspan = 5;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("FOLIO", baseFontLite));
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Table.AddCell(cell);

            cell = new PdfPCell(new Phrase(writer.PageNumber.ToString("000"), baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            //Row 2
            cell = new PdfPCell(new Phrase(this.Titulo1, baseFontBig));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.Colspan = 5;
            Table.AddCell(cell);
            // add a image
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(this.ImagePath + "logo.jpg");
            PdfPCell imageCell = new PdfPCell(jpg);
            imageCell.Colspan = 2; // either 1 if you need to insert one cell
            imageCell.Border = 0;
            imageCell.HorizontalAlignment = Element.ALIGN_CENTER;
            imageCell.Rowspan = 6;
            Table.AddCell(imageCell);
            //Row 3
            cell = new PdfPCell(new Phrase("R.Social:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.Cuenta.Nombre, baseFontLite));
            cell.Colspan = 2;
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Cuenta:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.Cuenta.Id, baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            //Row 4
            cell = new PdfPCell(new Phrase("Domicilio:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.Cuenta.Domicilio, baseFontLite));
            cell.Colspan = 2;
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("CUIT:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.Cuenta.NumeroDocumento, baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            //Row 5
            cell = new PdfPCell(new Phrase("Localidad:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.Cuenta.CodigoPostal.Trim() + "-" + this.Cuenta.Localidad.Trim() + "-" + this.Cuenta.Provincia.Trim(), baseFontLite));
            cell.Colspan = 2;
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            //Row 6
            cell = new PdfPCell(new Phrase("email:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("", baseFontLite));
            cell.Colspan = 6;
            cell.Border = 0;
            Table.AddCell(cell);
            //Row 7
            cell = new PdfPCell(new Phrase("Fecha Desde:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.FechaDesde.ToShortDateString(), baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Fecha Hasta:", baseFontLite));
            cell.Border = 0;
            Table.AddCell(cell);
            cell = new PdfPCell(new Phrase(this.FechaHasta.ToShortDateString(), baseFontLite));
            cell.Border = 0;
            cell.Colspan = 4;
            Table.AddCell(cell);

            //Table.TotalWidth = document.PageSize.Width - 80f;
            Table.TotalWidth = document.PageSize.Width - 95f;
            Table.WidthPercentage = 70;
            //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;    

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            //Table.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            Table.WriteSelectedRows(0, -1, 65, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value

            //Move the pointer and draw line to separate header section from rest of page
            //cb.MoveTo(40, document.PageSize.Height - 100);
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
            //cb.Stroke();

            //Move the pointer and draw line to separate footer section from rest of page
            //cb.MoveTo(40, document.PageSize.GetBottom(50));
            cb.MoveTo(40, document.PageSize.GetBottom(50));
            cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
            cb.Stroke();


        }
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(400, 150);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();
        }
    }
    
}
