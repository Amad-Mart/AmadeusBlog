using AmadeusBlog.Data;
using AmadeusBlog.Models;
using AmadeusBlog.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Services
{
    public class SearchService
    {
        readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IOrderedQueryable<Post> SearchContent(string searchString)
        {
            //step 1: Get an IQueryable that contains all the Posts in the event that
            //that user does not supply a serach string
            var result = _context
                            .Post
                            .Where(p =>
                                p.PublishState ==
                                PublishState.ProductionReady);

            searchString = searchString.ToLower();
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(p => 
                           p.Title.ToLower().Contains(searchString) ||
                           p.Abstract.ToLower().Contains(searchString) ||
                           p.Content.ToLower().Contains(searchString) 
                           /*p.Comments.Any(c => c.Moderated == null &&
                                               c.Body.Contains(searchString) ||
                                               c.ModeratedBody.Contains(searchString) ||
                                               c.Author.FirstName.Contains(searchString) ||
                                               c.Author.LastName.Contains(searchString) ||
                                               c.Author.DisplayName.Contains(searchString) ||
                                               c.Author.Email.Contains(searchString)
                           //to not show comments that have been moded, they could be nasty
                           //p.Comments.Any(c => c.Moderated == null && 
                           //                    c.Body.Contains(searchString) ||
                           //                    c.ModeratedBody.Contains(searchString)) */
                           );
            }
            return result.OrderByDescending(p => p.Created);
        }
    }
}
