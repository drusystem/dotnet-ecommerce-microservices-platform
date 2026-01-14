using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/catalog/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = "Producto no encontrado" });

        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(Guid categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productDtos);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
    {
        var product = _mapper.Map<Product>(createDto);
        var createdProduct = await _productRepository.CreateAsync(product);
        
        var productDto = _mapper.Map<ProductDto>(await _productRepository.GetByIdAsync(createdProduct.Id));
        
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto updateDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return NotFound(new { message = "Producto no encontrado" });

        _mapper.Map(updateDto, existingProduct);
        existingProduct.Id = id;
        
        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        var productDto = _mapper.Map<ProductDto>(await _productRepository.GetByIdAsync(updatedProduct.Id));
        
        return Ok(productDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _productRepository.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = "Producto no encontrado" });

        return NoContent();
    }
}