using System.ComponentModel.DataAnnotations;

namespace Filmes.API.Model
{
    public class Filmes{
        public int? Id {get; set;}
        [StringLength(100, ErrorMessage = "O nome do filme deve ter no máximo 100 caracteres")]
        public string? Nome {get;set;}
        [StringLength(100, ErrorMessage = "O gênero deve ter no máximo 100 caracteres")]
        public string? Genero {get;set;}
        [StringLength(100, ErrorMessage = "A plataforma deve ter no máximo 100 caracteres")]
        public string? Plataforma {get;set;}
        [Range(1895, 2024, ErrorMessage = "O ano deve estar entre 1895 e 2024")]
        public int? Ano {get;set;}
        public bool IsAssistido {get;set;} = false;
    }
}