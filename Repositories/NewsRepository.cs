using fantasy_hoops.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using fantasy_hoops.Dtos;
using fantasy_hoops.Models;
using fantasy_hoops.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace fantasy_hoops.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly GameContext _context;

        public NewsRepository()
        {
            _context = new GameContext();
        }

        public Dictionary<String, List<NewsDto>> GetNews(int start, int count)
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
                    Type = news.Type.GetDisplayName(),
                    Title = news.Title,
                    Paragraphs = news.Paragraphs
                        .OrderBy(p => p.ParagraphNumber)
                        .Select(y => y.Content)
                        .ToList(),
                    Date = news.Date.ToString("MM/dd/yyyy"),
                    hTeam = news.hTeam.Abbreviation,
                    vTeam = news.vTeam.Abbreviation
                })
                .GroupBy(news => news.Type)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public List<NewsDto> GetPreviews(int start, int count)
        {
            return _context.News
                .Where(news => news.Type == NewsType.PREVIEW)
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
                    Type = news.Type.GetDisplayName(),
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

        public List<NewsDto> GetRecaps(int start, int count)
        {
            return _context.News
                .Where(news => news.Type == NewsType.RECAP)
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
                    Type = news.Type.GetDisplayName(),
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