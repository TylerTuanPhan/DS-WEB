﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using WebXeHoi.Models;
using WebXeHoi.Repositories;

namespace WebXeHoi.Controllers
{

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm từ repository
            var products = await _productRepository.GetAllAsync();

            // Lấy danh sách các category từ cơ sở dữ liệu
            var categories = await _categoryRepository.GetAllAsync();

            // Gán danh sách category vào ViewBag.Categories
            ViewBag.Categories = categories;

            // Trả về view với danh sách sản phẩm
            return View(products);
        }



        // Xử lý thêm sản phẩm mới
        public async Task<IActionResult> Create(Product product ,IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    product.ImageUrl = await SaveImage(ImageUrl);
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        private async Task<string> SaveImage(IFormFile Image)
        {
            var savePath = Path.Combine("wwwroot/images", Image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await Image.CopyToAsync(fileStream);
            }
            return "/images/" + Image.FileName; // Trả về đường dẫn tương đối
        }

        // Hien thi form edit san pham
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Xu li cap nhat san pham
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile ImageUrl)
        {
            if (ModelState.IsValid)
            {
                if (ImageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    product.ImageUrl = await SaveImage(ImageUrl);
                }
                await _productRepository.UpdateAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //Hien thi chi tiet 1 san pham
        public async Task<IActionResult> Details(int id)
        {
            var product =  await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
