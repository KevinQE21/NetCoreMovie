namespace back_end.Repositorios
{
    using back_end.Entidades;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RepositorioEnMemoria: IRepositorio
    {
        private List<Genero> _generos;

        public RepositorioEnMemoria()
        {
            _generos = new List<Genero>()
            {
                new Genero(){ Id = 1, Nombre = "Accion" },
                new Genero(){ Id = 2, Nombre = "Comedia" }
            };
        }

        public List<Genero> ObtenerTodosLosGeneros()
        {
            return _generos;
        }

        public async Task<Genero> ObtenerPorId(int Id)
        {
            await Task.Delay(1);

            return _generos.FirstOrDefault(x => x.Id == Id);
        }
    }
}
