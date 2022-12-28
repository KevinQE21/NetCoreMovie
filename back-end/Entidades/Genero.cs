namespace back_end.Entidades
{
    using back_end.Validaciones;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class Genero: IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        //[JsonPropertyName("Nombre")]
        //[PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula", new string[] { nameof(Nombre) });
                }
            }
        }
    }
}
