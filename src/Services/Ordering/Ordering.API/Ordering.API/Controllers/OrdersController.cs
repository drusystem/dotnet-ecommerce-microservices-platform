using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.DTOs;
using Ordering.Application.Queries;

namespace Ordering.API.Controllers;

[ApiController]
[Route("api/ordering/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los pedidos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
    {
        var query = new GetAllOrdersQuery();
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    /// <summary>
    /// Obtener un pedido por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await _mediator.Send(query);

        if (order == null)
            return NotFound(new { message = "Pedido no encontrado" });

        return Ok(order);
    }

    /// <summary>
    /// Obtener pedidos de un usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByUser(string userId)
    {
        var query = new GetOrdersByUserQuery(userId);
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    /// <summary>
    /// Crear un nuevo pedido
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var command = new CreateOrderCommand(
            createOrderDto.UserId,
            createOrderDto.UserEmail,
            createOrderDto.ShippingAddress,
            createOrderDto.Items
        );

        var order = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Confirmar un pedido
    /// </summary>
    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmOrder(Guid id)
    {
        var command = new ConfirmOrderCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Pedido no encontrado o no se pudo confirmar" });

        return Ok(new { message = "Pedido confirmado exitosamente" });
    }

    /// <summary>
    /// Marcar pedido como enviado
    /// </summary>
    [HttpPost("{id}/ship")]
    public async Task<ActionResult> ShipOrder(Guid id)
    {
        var command = new ShipOrderCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Pedido no encontrado o no se pudo marcar como enviado" });

        return Ok(new { message = "Pedido marcado como enviado" });
    }

    /// <summary>
    /// Marcar pedido como entregado
    /// </summary>
    [HttpPost("{id}/deliver")]
    public async Task<ActionResult> DeliverOrder(Guid id)
    {
        var command = new DeliverOrderCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Pedido no encontrado o no se pudo marcar como entregado" });

        return Ok(new { message = "Pedido marcado como entregado" });
    }

    /// <summary>
    /// Cancelar un pedido
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id)
    {
        var command = new CancelOrderCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = "Pedido no encontrado o no se pudo cancelar" });

        return Ok(new { message = "Pedido cancelado" });
    }

    /// <summary>
    /// Eliminar un pedido
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOrder(Guid id)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await _mediator.Send(query);

        if (order == null)
            return NotFound(new { message = "Pedido no encontrado" });

        // Aquí podrías agregar lógica para eliminar a través de un comando
        // Por ahora solo validamos que existe

        return NoContent();
    }
}