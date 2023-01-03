using AutoMapper;
using back_end.DTOs;
using back_end.Entidades;
using back_end.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorAzureStorage almacenadorAzureStorage;
        private readonly string contenedor = "actores";

        public ActorController(ApplicationDBContext context, IMapper mapper, IAlmacenadorAzureStorage almacenadorAzureStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorAzureStorage = almacenadorAzureStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Actores.AsQueryable();
            await HttpContext.InsertarParemetrosPaginacionEnCabecera(queryable);
            var actores = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<ActorDTO>>(actores);
        }

        //Restriccion de ruta Id:int se hace que Id sea un int estricto
        [HttpGet("{Id:int}")]
        public async Task<ActionResult<ActorDTO>> Get(int Id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == Id);

            if (actor == null)
            {
                return NotFound();
            }

            return mapper.Map<ActorDTO>(actor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if(actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorAzureStorage.GuadarArchivo(this.contenedor, actorCreacionDTO.Foto);
            }

            context.Add(actor);
            await context.SaveChangesAsync(); 
            return NoContent();
        }

        [HttpPut("{Id:int}")]
        public async Task<ActionResult> Put(int Id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == Id);

            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionDTO, actor);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorAzureStorage.EditarArchivo(this.contenedor, actorCreacionDTO.Foto, actor.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            context.Remove(actor);
            await context.SaveChangesAsync();

            await almacenadorAzureStorage.BorrarArchivo(actor.Foto, this.contenedor);

            return NoContent();
        }
    }
}
