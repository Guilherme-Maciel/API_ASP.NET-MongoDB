using APImongodb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using APImongodb.Entities;

namespace APImongodb.Controllers
{
    [Route("api/v1/[controller]")]
    //habilita recursos à API
    [ApiController]
    public class CatalogController : ControllerBase
    {


        public readonly IProductRepository _repository;

        public CatalogController(IProductRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int id)
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            if (category is null)
            {
                return BadRequest("Invalid Category");
            }
            var products = await _repository.GetProductsByCategory(category);
            return Ok(products);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product is null)
            {
                return BadRequest("Invalid Product");
            }
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }
        [HttpPut]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            if (product is null)
                return BadRequest("Invalid Product");

            return Ok(await _repository.UpdateProduct(product));
        }
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }






    }

}

