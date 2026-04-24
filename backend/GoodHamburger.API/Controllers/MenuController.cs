using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Lista todos os itens do cardápio com preços e tipos.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var items = _menuService.GetAllItems();
        return Ok(items);
    }
}
