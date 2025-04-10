using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using WebApp.MVC.Models;

namespace WebApp.MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync("api/product/get-all");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);
                return View(products);
            }
            return View(new List<ProductViewModel>());
        }

        public async Task<IActionResult> Details(string id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync($"api/product/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductViewModel>(data);
                var category = await GetCategoryById(product?.CategoryId);
                if (product != null && category != null)
                {
                    product.CategoryName = category.Name;
                }
                return View(product);
            }

            return NotFound();
        }

        public async Task<IActionResult> Create()
        {
            var selectListCategory = await GetSelectListOfCategory();
            if (selectListCategory != null)
            {
                ViewBag.Categories = selectListCategory;
            }
            else
            {
                ViewBag.Categories = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            product.Id = Guid.NewGuid().ToString();
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("ProductApi");
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/product", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync($"api/product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductViewModel>(data);

                var selectListCategory = await GetSelectListOfCategory();
                if (selectListCategory != null)
                {
                    ViewBag.Categories = selectListCategory;
                }
                else
                {
                    ViewBag.Categories = new SelectList(new List<SelectListItem>(), "Value", "Text");
                }

                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("ProductApi");
                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/product/{product.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync($"api/product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductViewModel>(data);
                return View(product);
            }
            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.DeleteAsync($"api/product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private async Task<CategoryViewModel?> GetCategoryById(string? id)
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync($"api/categoryproduct/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<CategoryViewModel>(data);
                return category;
            }

            return null;
        }
        private async Task<List<SelectListItem>?> GetSelectListOfCategory()
        {
            var client = _httpClientFactory.CreateClient("ProductApi");
            var response = await client.GetAsync("api/categoryproduct");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var categoriesData = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);

                return categoriesData?.Select(c => new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();
            }

            return null;
        }
    }
}