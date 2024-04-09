using Domain;
using Infrasctructure.Persistence;
using ReinventedPuembo.Interfaces;
using ReinventedPuembo.Models;
using SelectPdf;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Drawing.Imaging;
using System.Drawing;
using BarcodeLib;
using WkHtmlToPdfDotNet;
using System.Diagnostics;

namespace ReinventedPuembo.AccesoDatos
{

    public class DocumentoRepositorio:IDocumento
    {
        private readonly ApplicationDbContext context;
        private readonly string _plantillasRoute;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
       

        public DocumentoRepositorio(ApplicationDbContext _context, IMapper _mapper, IWebHostEnvironment environment, IConfiguration config)
        {
            context = _context;
            mapper = _mapper;
            _environment = environment;
            _config = config;
            _plantillasRoute = Path.Combine(_environment.ContentRootPath, "wwwroot","Plantillas");
            
        }

          

       
        public string ImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }

        public async Task<byte[]> ObtenerPlantilla(string nombrePlantilla)
        {
            var filePath = Path.Combine(_plantillasRoute, nombrePlantilla);
            var archivo = await File.ReadAllBytesAsync(filePath);
            return archivo;
        }

        public async Task<string> GenerarPDFcontrato(string textoTopdf, string cedulaEstudiante, string codSucursal, string CicloEscolar)
        {
            var nombrePDF = cedulaEstudiante + "_" + CicloEscolar  + "_" + codSucursal + "_Contrato.pdf";
            var nombreHtml = cedulaEstudiante + "_" + CicloEscolar + "_" + codSucursal + "_Contrato.html";
            var rutaPDF = Path.Combine(_environment.ContentRootPath, "wwwroot", "DocsLegales",nombrePDF);
            var rutaHtml = Path.Combine(_environment.ContentRootPath, "wwwroot", "DocsLegales", nombreHtml);
            try
            {
               
                //HtmlToPdf converter = new HtmlToPdf();
                //PdfDocument doc = new PdfDocument();
                                

                    //converter.Options.PdfPageSize = PdfPageSize.A4;
                    //converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    //converter.Options.MarginBottom = 55;
                    //converter.Options.MarginLeft = 55;
                    //converter.Options.MarginRight = 55;
                    //converter.Options.MarginTop = 30;
                   // doc = converter.ConvertHtmlString(textoTopdf);
                Console.WriteLine(rutaPDF);
                //doc.Save(rutaPDF);
                //doc.Close();

                //         var converter = new SynchronizedConverter(new PdfTools());

                //         var doc = new HtmlToPdfDocument()
                //         {
                //             GlobalSettings = {
                //        ColorMode = WkHtmlToPdfDotNet.ColorMode.Color,
                //         Orientation = Orientation.Landscape,
                //         PaperSize = PaperKind.A4,
                //         Out=rutaPDF
                //      },
                //             Objects = {
                // new ObjectSettings() {
                //     PagesCount = true,
                //     HtmlContent = textoTopdf,
                //     WebSettings = { DefaultEncoding = "utf-8" },
                //     HeaderSettings = { FontSize = 9, Line = true, Spacing = 2.812 }
                // }
                //}
                //         };

                //         converter.Convert(doc);

            

                if (!System.IO.File.Exists(rutaHtml))
                {
                    Console.WriteLine($"{nombreHtml} file not found!");
                    using (StreamWriter sw = File.CreateText(rutaHtml))
                    {
                     await   sw.WriteAsync(textoTopdf);
                    }
                }
                string dirWin = "C:\\Users\\developer\\Desktop\\SGI-MatScho\\ReinventedPuembo\\wkhtmltopdf\\bin\\wkhtmltopdf.exe";
                string dirlinux = "/usr/bin/wkhtmltopdf";
                var processStartInfo = new ProcessStartInfo
                {
                    // Pass the path of the wkhtmltopdf executable.
                    FileName = dirlinux,
                    Arguments = $"{rutaHtml} {rutaPDF}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                Console.WriteLine($"Starting process with FileName: {processStartInfo.FileName} and Arguments: {processStartInfo.Arguments}");

                using (var process = Process.Start(processStartInfo))
                {
                    process?.WaitForExit();
                    if (process?.ExitCode == 0)
                    {
                        Console.WriteLine("HTML to PDF conversion successful!");
                        return nombrePDF;
                    }
                    else
                    {
                        Console.WriteLine("HTML to PDF conversion failed!");
                        Console.WriteLine(process?.StandardOutput.ReadToEnd());
                        return "Error al generar el archivo: " + nombrePDF;
                    }
                }



             
                
            }
            catch(Exception e)
            {
                
                return "Error al generar el archivo: " + nombrePDF;
            }
        }

    }
}
