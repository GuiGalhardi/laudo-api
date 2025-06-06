using LaudoApi.Constantes;
using LaudoApi.Models;  // Importa o modelo LaudoRequest
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
            if (string.IsNullOrEmpty(temp) || !Referencias.ValidTemperamentos.Contains(temp))
                return BadRequest($"Temperamento inválido. Deve ser um de: s, f, m ou c.");

            if (!Referencias.ValidEneagramas.Contains(ene))
                return BadRequest("Eneagrama inválido. Deve ser um inteiro de 1 a 9.");

            var nomeTemperamento = $"temperamento-{temp}.md";
            var nomeEneagrama = $"eneagrama-{ene}.md";

            // Caminho até a pasta onde os arquivos Markdown estão armazenados
            var markdownDir = Path.Combine(_env.ContentRootPath, "Markdowns");
            var pathTemp = Path.Combine(markdownDir, nomeTemperamento);
            var pathEne = Path.Combine(markdownDir, nomeEneagrama);

            // Verifica se os arquivos existem
            if (!System.IO.File.Exists(pathTemp))
                return NotFound($"Arquivo '{nomeTemperamento}' não encontrado.");
            if (!System.IO.File.Exists(pathEne))
                return NotFound($"Arquivo '{nomeEneagrama}' não encontrado.");

            string conteudoTemp;
            string conteudoEne;
            try
            {
                // Lê os arquivos Markdown
                conteudoTemp = await System.IO.File.ReadAllTextAsync(pathTemp, Encoding.UTF8);
                conteudoEne = await System.IO.File.ReadAllTextAsync(pathEne, Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                return StatusCode(500, $"Erro ao ler arquivos: {ioEx.Message}");
            }

            // Concatena os conteúdos dos arquivos
            var conteudoFinal = conteudoTemp.TrimEnd() + "\n\n" + conteudoEne.TrimEnd() + "\n";

            // Converte o conteúdo para bytes e retorna como arquivo para download
            var bytesLaudo = Encoding.UTF8.GetBytes(conteudoFinal);
            return File(bytesLaudo, "text/markdown", "laudo.md");
        }
    }
}
