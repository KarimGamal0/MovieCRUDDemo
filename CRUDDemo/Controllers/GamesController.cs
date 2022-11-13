using CRUDDemo.Models;
using CRUDDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDDemo.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IToastNotification _toastNotification;
        private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public GamesController(ApplicationDbContext context/*, IToastNotification toastNotification*/)
        {
            _context = context;
            //_toastNotification = toastNotification; 
        }

        public async Task<IActionResult> Index()
        {
            var Games = await _context.Games.OrderByDescending(G => G.Rate).ToListAsync();
            return View(Games);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new GameFormViewModel()
            {
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("GameForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("GameForm", model);
            }

            var files = Request.Form.Files;
            if (!files.Any())
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please select movie poster!");
                return View("GameForm", model);
            }

            var poster = files.FirstOrDefault();
            var allowedExtensions = new List<string> { ".jpg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only .PNG .JPG are allowed!");
                return View("GameForm", model);
            }

            if (poster.Length > _maxAllowedPosterSize)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB");
                return View("GameForm", model);
            }

            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            var Games = new Game
            {
                Title = model.Title,
                GenreId = model.GenreId,
                Year = model.Year,
                Rate = model.Rate,
                StoryLine = model.StoryLine,
                Poster = dataStream.ToArray()
            };

            _context.Games.Add(Games);
            _context.SaveChanges();

            //_toastNotification.AddSuccessToastMessage("Game Created Successfully");

            model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var Game = await _context.Games.FindAsync(id);

            if (Game == null)
                return NotFound();

            var viewModel = new GameFormViewModel()
            {
                Id = Game.Id,
                Title = Game.Title,
                GenreId = Game.GenreId,
                Rate = Game.Rate,
                Year = Game.Year,
                StoryLine = Game.StoryLine,
                Poster = Game.Poster,
                Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("GameForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GameFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                return View("GameForm", model);
            }

            var Game = await _context.Games.FindAsync(model.Id);

            if (Game == null)
                return NotFound();

            var files = Request.Form.Files;
            if (files.Any())
            {
                var poster = files.FirstOrDefault();

                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                model.Poster = dataStream.ToArray();

                if (!_allowedExtensions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only .PNG .JPG are allowed!");
                    return View("GameForm", model);
                }

                if (poster.Length > _maxAllowedPosterSize)
                {
                    model.Genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster cannot be more than 1 MB");
                    return View("GameForm", model);
                }

                Game.Poster = dataStream.ToArray();
            }

            Game.Title = model.Title;
            Game.GenreId = model.GenreId;
            Game.Year = model.Year;
            Game.Rate = model.Rate;
            Game.StoryLine = model.StoryLine;

            _context.SaveChanges();

            //_toastNotification.AddSuccessToastMessage("Game Updated Successfully");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var game = await _context.Games.Include(G =>G.Genre).SingleOrDefaultAsync(G=>G.Id == id);

            if (game == null)
                return NotFound();

            return View(game);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            _context.SaveChanges();


            //return Ok();
            return RedirectToAction("Index");
        }
    }
}
