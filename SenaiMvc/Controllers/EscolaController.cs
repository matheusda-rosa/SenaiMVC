using System.Reflection;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SenaiMvc.Models.Escola;
using SenaiMvc.Service.Interfaces;

namespace SenaiMvc.Controllers
{
    public class EscolaController : Controller
    {

        private readonly IApiService _apiService;

        public EscolaController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var escolas = await _apiService.GetAsync<List<EscolaModel>>("Escola/buscar-todos");
            return View(escolas);
        }

        [HttpGet]
        public async Task<IActionResult> Form()
        {
            var model = new EscolaModel();
            await AlimentarEstados(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Form(EscolaModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Endereco.Id == null)
                    model.Endereco.Id = 0;
                var retorno = await _apiService.PostAsync<EscolaModel>("Escola", model);
                return Redirect("Index");
            }
            await AlimentarEstados(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(long id)
        {
            var model = await _apiService.GetAsync<EscolaModel>($"Escola/PegarPorId?id={id}");
            if (model.Endereco == null)
                model.Endereco = new EnderecoModel();
            await AlimentarEstados(model);
            if (!string.IsNullOrEmpty(model.Endereco.Estado))
            {
                await AlimentarCidade(model, model.Endereco.Estado);
            }
            return View("Form", model);
        }
        [HttpGet]
        public async Task<IActionResult> Excluir(long id)
        {
            var retorno = await _apiService.DeleteAsync($"Escola?id={id}");
            return RedirectToAction("Index");
        }

        private async Task AlimentarEstados(EscolaModel model)
        {
            var estados = await _apiService.PegarEstados<EstadoIBGE>();
            model.Estados = estados.OrderBy(e => e.Nome)
                .Select(e => new SelectListItem
                {
                    Value = e.Sigla,
                    Text = e.Nome
                })
                .ToList();
        }

        private async Task AlimentarCidade(EscolaModel model, string uf)
        {
            var cidades = await _apiService.AlimentarCidades<CidadeIBGE>(uf);
            model.Cidades = cidades
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome,
                    Selected = c.Id.ToString() == model.Endereco.Cidade.ToString()
                }).ToList();
        }


        [HttpGet]
        public async Task<IActionResult> ObterCidadesPorUF(string uf)
        {            
            var cidades = await _apiService.AlimentarCidades<CidadeIBGE>(uf);
            var resultado = cidades.OrderBy(c => c.Nome).Select(c => new { id = c.Id, nome = c.Nome }).ToList();
            return Json(resultado);            
        }
    }
}
