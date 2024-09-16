// Razor Component (ProductList.razor)
@page "/products"
@inject IProductRepository Repository
@inject NavigationManager NavigationManager

<h1>Product List</h1>

@if (products == null)
{
    <p>Loading...</p>
}
else
{
    <table>
        <thead>
            <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Price</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var product in products)
            {
                <tr>
                    <td>@product.Id</td>
                    <td>@product.Name</td>
                    <td>@product.Price</td>
                    <td>
                        <button @onclick="() => EditProduct(product.Id)">Edit</button>
                        <button @onclick="() => DeleteProduct(product.Id)">Delete</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

<button @onclick="AddProduct">Add New Product</button>

@code {
    private List<Product> products;

    protected override async Task OnInitializedAsync()
    {
        await RefreshProducts();
    }

    private async Task RefreshProducts()
    {
        products = (await Repository.GetAllProducts()).ToList();
    }

    private async Task AddProduct()
    {
        var newProduct = new Product { Name = "New Product", Price = 0 };
        await Repository.AddProduct(newProduct);
        await RefreshProducts();
    }

    private void EditProduct(int id)
    {
        NavigationManager.NavigateTo($"/edit-product/{id}");
    }

    private async Task DeleteProduct(int id)
    {
        await Repository.DeleteProduct(id);
        await RefreshProducts();
    }
}

// Controller (ProductsController.cs)
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _repository.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repository.GetProductById(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        var createdProduct = await _repository.AddProduct(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();

        var updated = await _repository.UpdateProduct(product);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _repository.DeleteProduct(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

// IProductRepository interface (for reference)
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProducts();
    Task<Product> GetProductById(int id);
    Task<Product> AddProduct(Product product);
    Task<bool> UpdateProduct(Product product);
    Task<bool> DeleteProduct(int id);
}
