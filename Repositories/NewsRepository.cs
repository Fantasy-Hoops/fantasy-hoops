using fantasy_hoops.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Repositories
{
    public class NewsRepository : INewsRepository
    {

        private readonly GameContext _context;

        public NewsRepository()
        {
            _context = new GameContext();
        }

        public List<NewsDto> GetNews(int start, int count)
        {
            return _context.News
                .OrderByDescending(news => news.Date)
                .Skip(start)
                .Take(count)
                .Include(news => news.Paragraphs)
                .Include(news => news.hTeam)
                .Include(news => news.vTeam)
                .AsEnumerable()
                .Select(news => new NewsDto
                {
                    Id = news.NewsID,
                    Title = news.Title,
                    Paragraphs = news.Paragraphs
                                    .OrderBy(p => p.ParagraphNumber)
                                    .Select(y => y.Content)
                                    .ToList(),
                    Date = news.Date.ToString("MM/dd/yyyy"),
                    hTeam = news.hTeam.Abbreviation,
                    vTeam = news.vTeam.Abbreviation
                })
                .ToList();
        }

    }
}
