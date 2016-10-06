﻿using System.Runtime;
using AlbumViewerBusiness;
using System;
using System.Linq;

using System.Reflection;
using System.Threading.Tasks;
using AlbumViewerAspNet5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicStoreVNext
{
    public class AlbumViewerMvcController : Controller
    {
        AlbumViewerContext context;
        private readonly IServiceProvider serviceProvider;

        public AlbumViewerMvcController(AlbumViewerContext ctx, IServiceProvider svcProvider)
        {
            context = ctx;
            serviceProvider = svcProvider;
            //this.environment = environment;
        }

        
        public async Task<ActionResult> Index()
        {
            return await Albums();
        }

        [Route("albums")]
        public async Task<ActionResult> Albums()
        {
            
            var result = await context.Albums
                .Include(ctx => ctx.Tracks)
                .Include(ctx => ctx.Artist)
                .OrderBy(alb => alb.Title)
                .ToListAsync();

            return View("Albums", result);
        }


        [Route("album/{id:int}")]
        public async Task<ActionResult> Album(int id)
        {
            var albumRepo = new AlbumRepository(context);
            var album = albumRepo.Load(id);

            return View("Album", album);
        }

        [Route("artists")]
        public async Task<ActionResult> Artists()
        {
            var artists = await context.Artists
               .OrderBy(art => art.ArtistName)
               .Select(art => new ArtistWithAlbumCount()
               {
                   ArtistName = art.ArtistName,
                   Description = art.Description,
                   ImageUrl = art.ImageUrl,
                   Id = art.Id,
                   AmazonUrl = art.AmazonUrl,
                   AlbumCount = context.Albums.Count(alb => alb.ArtistId == art.Id)
               })
               .ToAsyncEnumerable()
               .ToList();

            return View("Artists", artists);
        }

 

        [HttpGet,HttpPost]
        public ActionResult TagHelpers(Album album = null)
        {
            if (album == null || (string.IsNullOrEmpty(album.Description) && string.IsNullOrEmpty(album.Title)))
            {
                album = new Album()
                {
                    Id = 1,
                    Title = "Highway to Hell",
                    Description = "AC/DC's best"
                };
            }
            else
            {

                if (ModelState.IsValid)
                {
                    ModelState.Remove("Title");
                    album.Title = album.Title + " updated.";
                }
            }

            return View(album);
        }



    }
}
