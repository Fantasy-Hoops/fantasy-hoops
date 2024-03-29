﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using fantasy_hoops.Database;
using fantasy_hoops.Helpers;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Jobs
{
    public class PhotosJob : ICronJob
    {
        private readonly GameContext _context;

        const string photosDir = "./ClientApp/build/content/images/players/";
        const string logosDir = "./ClientApp/build/content/images/logos/";

        public PhotosJob()
        {
            _context = new GameContext();
        }

        private void ExtractLogos()
        {
            if (!Directory.Exists(logosDir))
                Directory.CreateDirectory(logosDir);

            foreach (var team in _context.Teams)
            {
                string teamAbbr = team.Abbreviation;
                string remoteFileUrl =
                    "http://i.cdn.turner.com/nba/nba/assets/logos/teams/secondary/web/" + teamAbbr + ".svg";
                string localFileName = "./ClientApp/build/content/images/logos/" + teamAbbr + ".svg";
                SavePhoto(localFileName, remoteFileUrl);
            }
        }

        private void ExtractPlayerPhotos()
        {
            if (!Directory.Exists(photosDir))
                Directory.CreateDirectory(photosDir);

            foreach (var player in _context.Players)
            {
                int personId = player.NbaID;
                string remoteFileUrl =
                    "https://ak-static.cms.nba.com/wp-content/uploads/headshots/nba/latest/260x190/" + personId + ".png";
                string localFileName = "./ClientApp/build/content/images/players/" + personId + ".png";
                SavePhoto(localFileName, remoteFileUrl);
            }
        }

        private void SavePhoto(string localFile, string urlFile)
        {
            byte[] content;
            WebResponse response = CommonFunctions.Instance.GetResponse(urlFile);
            if (response == null)
                return;
            Stream stream = response.GetResponseStream();
            using (BinaryReader br = new BinaryReader(stream))
            {
                content = br.ReadBytes(500000);
                br.Close();
            }
            response.Close();

            if (!NeedDownload(localFile, content))
                return;

            FileStream fs = new FileStream(localFile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(content);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }

        private bool NeedDownload(string localFile, byte[] urlBytes)
        {
            if (!File.Exists(localFile))
                return true;

            byte[] localFileBytes = File.ReadAllBytes(localFile);
            if (localFileBytes.Length != urlBytes.Length)
                return true;

            if (localFileBytes.SequenceEqual(urlBytes))
                return false;

            return false;
        }

        private async Task DeleteNotifications()
        {
            var weekAgo = DateTime.Today.ToUniversalTime().AddDays(-7);
            await _context.Notifications
                .Where(n => n.DateCreated < weekAgo)
                .ForEachAsync(notification => _context.Notifications.Remove(notification));
            await _context.SaveChangesAsync();
        }

        public void Execute()
        {
            ExtractLogos();
            ExtractPlayerPhotos();
            Task.Run(() => DeleteNotifications());
        }
    }
}