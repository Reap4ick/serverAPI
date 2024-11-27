/*using ApiStore.Data;
using ApiStore.Interfaces;
using ApiStore.Models.Post;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApiStoreDbContext _context;
        private readonly IImageHulk _imageHulk;
        private readonly IMapper _mapper;

        public PostsController(ApiStoreDbContext context, IImageHulk imageHulk, IMapper mapper)
        {
            _context = context;
            _imageHulk = imageHulk;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            var viewModels = _mapper.Map<List<PostItemViewModel>>(posts);
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            var viewModel = _mapper.Map<PostItemViewModel>(post);
            return Ok(viewModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] PostEditDto model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            _mapper.Map(model, post);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Post updated successfully" });
        }
    }
}
*/




using ApiStore.Data;
using ApiStore.Interfaces;
using ApiStore.Models;
using ApiStore.Models.Post;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ApiStore.Data.Entities;

namespace ApiStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApiStoreDbContext _context;
        private readonly IImageHulk _imageHulk;
        private readonly IMapper _mapper;

        public PostsController(ApiStoreDbContext context, IImageHulk imageHulk, IMapper mapper)
        {
            _context = context;
            _imageHulk = imageHulk;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

            var viewModels = _mapper.Map<List<PostItemViewModel>>(posts);
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            var viewModel = _mapper.Map<PostItemViewModel>(post);
            return Ok(viewModel);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] PostEditDto model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }

            _mapper.Map(model, post);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Post updated successfully" });
        }

        // Додано метод для наповнення бази даних
        [HttpPost("seed")]
        public async Task<IActionResult> SeedPosts()
        {
            // Перевіряємо, чи є дані в таблиці `Posts`
            if (await _context.Posts.AnyAsync())
            {
                return BadRequest(new { message = "Database already contains posts" });
            }

            var posts = new List<PostsEntity>
            {
                new PostsEntity
                {
                    Title = "How to Learn C#",
                    Body = "Start with the basics, then move to advanced concepts.",
                    DateCreated = DateTime.UtcNow.AddDays(-10)
                },
                new PostsEntity
                {
                    Title = "ASP.NET Core for Beginners",
                    Body = "Learn how to build web APIs with ASP.NET Core.",
                    DateCreated = DateTime.UtcNow.AddDays(-7)
                },
                new PostsEntity
                {
                    Title = "Entity Framework Core Tips",
                    Body = "Explore efficient database access patterns.",
                    DateCreated = DateTime.UtcNow.AddDays(-5)
                },
                new PostsEntity
                {
                    Title = "Understanding Dependency Injection",
                    Body = "Master DI to simplify your applications.",
                    DateCreated = DateTime.UtcNow.AddDays(-3)
                },
                new PostsEntity
                {
                    Title = "Top 5 C# Libraries",
                    Body = "Check out the most popular C# libraries.",
                    DateCreated = DateTime.UtcNow.AddDays(-1)
                }
            };

            // Додаємо записи до бази
            await _context.Posts.AddRangeAsync(posts);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Posts seeded successfully", count = posts.Count });
        }
    }
}
