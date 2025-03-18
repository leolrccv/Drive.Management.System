using Application.Contracts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using Markdig;
using Microsoft.AspNetCore.Http;

namespace Application.Converters;

public class MdToDocConverter : IFileConverter
{
    public Stream Convert(IFormFile file)
    {
        // Lê o conteúdo do Markdown
        // Lê o conteúdo do Markdown
        string markdownContent;
        using (var streamReader = new StreamReader(file.OpenReadStream()))
        {
            markdownContent = streamReader.ReadToEnd();
        }

        // Converte Markdown para HTML
        var htmlContent = Markdown.ToHtml(markdownContent);

        // Criar um MemoryStream para armazenar o documento
        var memoryStream = new MemoryStream();

        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();

            // Criando um processador de HTML para converter para OpenXML
            var converter = new HtmlConverter(mainPart);
            var paragraphs = converter.Parse(htmlContent);

            // Adiciona os parágrafos convertidos ao corpo do documento
            foreach (var para in paragraphs)
            {
                body.Append(para);
            }

            // Salva o documento
            mainPart.Document.Append(body);
            mainPart.Document.Save();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    //public Stream Convert(IFormFile file)
    //{
    //    string markdownText;
    //    using (var reader = new StreamReader(file.OpenReadStream()))
    //    {
    //        markdownText = reader.ReadToEnd();
    //    }

    //    // Converte Markdown para HTML usando Markdig
    //    var pipeline = new MarkdownPipelineBuilder().Build();
    //    string htmlContent = Markdown.ToHtml(markdownText, pipeline);

    //    // Cria um MemoryStream para o arquivo .docx
    //    var memoryStream = new MemoryStream();

    //    // Cria o documento Word
    //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
    //    {
    //        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
    //        mainPart.Document = new Document();
    //        Body body = mainPart.Document.AppendChild(new Body());

    //        // Adiciona o conteúdo HTML como parágrafos simples
    //        Paragraph para = body.AppendChild(new Paragraph());
    //        Run run = para.AppendChild(new Run());
    //        run.AppendChild(new Text(htmlContent)); // Simplificado: HTML puro como texto

    //        // Salva o documento
    //        mainPart.Document.Save();
    //    } // O 'using' fecha o WordprocessingDocument aqui, mas o MemoryStream ainda está válido

    //    // Reseta a posição do MemoryStream para o início
    //    memoryStream.Position = 0;
    //    return memoryStream;
    //}

    //public Stream Convert(IFormFile file)
    //{
    //    using var reader = new StreamReader(file.OpenReadStream());

    //    var markdownText = reader.ReadToEnd();

    //    var pipeline = new MarkdownPipelineBuilder().Build(); //ver se nao daria pra usar como singleton
    //    var htmlContent = Markdown.ToHtml(markdownText, pipeline);

    //    var memoryStream = new MemoryStream();
    //    using var wordDoc = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true);

    //    var mainPart = wordDoc.AddMainDocumentPart();
    //    mainPart.Document = new Document();
    //    var body = mainPart.Document.AppendChild(new Body());

    //    // Adiciona o conteúdo HTML como parágrafos simples
    //    var para = body.AppendChild(new Paragraph());
    //    var run = para.AppendChild(new Run());
    //    run.AppendChild(new Text(htmlContent)); // Simplificado: HTML puro como texto

    //    // Salva o documento
    //    mainPart.Document.Save();

    //    // Reseta a posição do MemoryStream para o início
    //    memoryStream.Position = 0;
    //    return memoryStream;
    //}
}