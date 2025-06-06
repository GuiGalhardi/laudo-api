using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LaudoApi.Models;

namespace LaudoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaudoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public LaudoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("gerar-laudo")]
        public async Task<IActionResult> GerarLaudo([FromBody] LaudoRequest request)
        {
            if (request == null)
                return BadRequest("Corpo da requisição inválido.");

            var temp = request.Temperamento?.Trim().ToLowerInvariant();
            var ene = request.Eneagrama;

            // Validações
            if (string.IsNullOrEmpty(temp) || !new[] { "s", "f", "m", "c" }.Contains(temp))
                return BadRequest("Temperamento inválido.");
            if (ene < 1 || ene > 9)
                return BadRequest("Eneagrama inválido. Deve ser de 1 a 9.");

            // Monta o nome do arquivo baseado nas entradas
            var nomeArquivo = $"natureza-{temp}-{ene}.md";

            // Caminho completo até a pasta Markdown
            var markdownDir = Path.Combine(_env.ContentRootPath, "Markdowns");
            var pathArquivo = Path.Combine(markdownDir, nomeArquivo);

            // Verifica se o arquivo existe
            if (!System.IO.File.Exists(pathArquivo))
                return NotFound($"Arquivo '{nomeArquivo}' não encontrado.");

            // Lê o conteúdo do arquivo
            string conteudoArquivo;
            try
            {
                conteudoArquivo = await System.IO.File.ReadAllTextAsync(pathArquivo, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                return StatusCode(500, $"Erro ao ler arquivo: {ioEx.Message}");
            }

            // Converte para bytes e retorna como FileResult
            var bytesLaudo = Encoding.UTF8.GetBytes(conteudoArquivo);
            return File(bytesLaudo, "text/markdown", "laudo.md");
        }
    }
}
