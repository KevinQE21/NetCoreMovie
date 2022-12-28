using back_end.Entidades;
using back_end.Filtros;
using back_end.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.Controllers
{
    [Route("api/generos")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController : ControllerBase
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger<Genero> logger;

        public GenerosController(IRepositorio repositorio, ILogger<Genero> logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        [HttpGet]
        //[ResponseCache(Duration = 60)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public List<Genero> Get()
        {
            logger.LogInformation("Vamos a mostrar los generos");
            return repositorio.ObtenerTodosLosGeneros();
        }

        //Restriccion de ruta Id:int se hace que Id sea un int estricto
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<Genero>> Get(int Id, [FromHeader] string nombre)
        {
            //ModelState son las validaciones cuando se hacen un binding 
            //if (ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            logger.LogDebug("Obteniendo un genero por el id");
            var genero = await repositorio.ObtenerPorId(Id);

            if (genero == null)
            {
                throw new ApplicationException($"El genero de ID {Id} no fue encontrado");
                logger.LogWarning($"No pudimos encontrar el genero de id {Id}");
                return NotFound();
            }

            return genero;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genero genero)
        {
            return NoContent();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genero genero)
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
    }
}
