using APICatalogo.Models;

namespace APICatalogo.DTOs.Mappings;

public static class CategoriaDTOMappingExtensions
{
    public static CategoriaDTO? ToCategoriaDTO(this Categoria categoria)
    {
        if (categoria is null) return null;
        return new CategoriaDTO
        {
            CategoriaId = categoria.CategoriaId,
            Nome = categoria.Nome,
            ImageUrl = categoria.ImageUrl
        };
    }

    public static Categoria? ToCategoria(this  CategoriaDTO categoriaDTO)
    {
        if(categoriaDTO is null) return null;
        return new Categoria
        {
            CategoriaId = categoriaDTO.CategoriaId,
            Nome = categoriaDTO.Nome,
            ImageUrl = categoriaDTO.ImageUrl
        };
    }

    public static IEnumerable<CategoriaDTO> ToCategoriaDTOList(this IEnumerable<Categoria> categorias)
    {
        if(categorias is null || !categorias.Any()) return new List<CategoriaDTO>();

        return categorias.Select(c => new CategoriaDTO
        {
            CategoriaId = c.CategoriaId,
            Nome = c.Nome,
            ImageUrl = c.ImageUrl
        }).ToList();
    }

}
