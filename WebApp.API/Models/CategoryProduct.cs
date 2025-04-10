using System;
using System.Collections.Generic;

namespace WebApp.API.Models;

public partial class CategoryProduct
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
