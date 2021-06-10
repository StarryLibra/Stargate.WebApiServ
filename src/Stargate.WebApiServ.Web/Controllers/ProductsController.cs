using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stargate.WebApiServ.Data.Models;
using Stargate.WebApiServ.Data.Repositories;

// For more information on archive 'Controller action return types in ASP.NET Core web API',
// visit https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-5.0
// sample code：https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/web-api/action-return-types/samples

namespace Stargate.WebApiServ.Web.Controllers
{
    /// <summary>
    /// 示例用的商品控制器
    /// </summary>
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsRepository _repository;

        /// <summary>构造函数</summary>
        /// <param name="repository">商品数据仓储</param>
        public ProductsController(ProductsRepository repository)
        {
            _repository = repository;
        }

        #region snippet_Get
        /// <summary>
        /// 获取商品。
        /// </summary>
        /// <returns>商品列表。</returns>
        [HttpGet]
        public List<Product> Get() => _repository.GetProducts();
        #endregion

        #region snippet_GetById
        /// <summary>
        /// 根据 ID 获取商品
        /// </summary>
        /// <param name="id">商品唯一标识符</param>
        /// <returns>相应的商品</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Product> GetById(int id)
        {
            if (!_repository.TryGetProduct(id, out var product))
            {
                return NotFound();
            }

            return product;
        }
        #endregion

        #region snippet_GetOnSaleProducts
        /// <summary>
        /// 获取在售商品（同步方式）
        /// </summary>
        /// <returns>在售商品列表</returns>
        [HttpGet("syncsale")]
        public IEnumerable<Product> GetOnSaleProducts()
        {
            var products = _repository.GetProducts();

            foreach (var product in products)
            {
                if (product.IsOnSale)
                {
                    yield return product;
                }
            }
        }
        #endregion

        #region snippet_GetOnSaleProductsAsync
        /// <summary>
        /// 获取在售商品（异步方式）
        /// </summary>
        /// <returns>在售商品列表</returns>
        [HttpGet("asyncsale")]
        public async IAsyncEnumerable<Product> GetOnSaleProductsAsync()
        {
            var products = _repository.GetProductsAsync();

            await foreach (var product in products)
            {
                if (product.IsOnSale)
                {
                    yield return product;
                }
            }
        }
        #endregion

        #region snippet_CreateAsync
        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="product">新商品</param>
        /// <returns>新增后的商品</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateAsync(Product product)
        {
            if (product.Description.Contains("XYZ Widget"))
            {
                return BadRequest();
            }

            await _repository.AddProductAsync(product);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        #endregion
    }
}
