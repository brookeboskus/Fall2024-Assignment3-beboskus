using System;
using Fall2024_Assignment3_beboskus.Models;
using Microsoft.AspNetCore.Mvc;
using Fall2024_Assignment3_beboskus.Data;
using Microsoft.EntityFrameworkCore;

namespace Fall2024_Assignment3_beboskus.Controllers
{
	public class ActorsController : Controller
	{
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        //POST: Actors/Create
        public async Task<IActionResult> Create([Bind("Name, Gender, Age, IMDBLink, PhotoUrl")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            return View(actor);
        }

        //GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FirstOrDefaultAsync(a => a.Id == id);
            if(actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        //POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();

            }

            _context.Actor.Remove(actor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the movie by ID and include the related actors
            var actor = await _context.Actor
                .Include(m => m.Movies)  // Fetch the actors associated with the movie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            // Create a ViewModel if necessary, or just pass the movie directly
            return View(actor);
        }

        //GET: Actors/Edit/{id
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the actor by ID
            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);  // Pass the actor to the view
        }

        // POST: Actors/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Gender, Age, IMDBLink, PhotoUrl")] Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);  // Update the actor in the database
                    await _context.SaveChangesAsync();  // Save changes
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect back to the actor list after edit
            }
            return View(actor);  // Return the actor with validation errors if the ModelState is invalid
        }


        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }

    }

}

