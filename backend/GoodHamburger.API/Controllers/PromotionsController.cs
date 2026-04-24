using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    /// <summary>
    /// Lista todas as promoções com nomes, descontos, condições e itens disponíveis.
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var promotions = _promotionService.GetAllPromotions();
        return Ok(promotions);
    }
}
