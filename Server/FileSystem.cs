using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static SharedDataClasses.Encryption;

namespace Server
{
    class FileSystem
    {
        private List<Score> scoreBoard { get; }

        public FileSystem()
        {
            scoreBoard = ReadFile();
        }

        public void WritetoFile()
        {
            string path = GetFilePath(true);
            string data = Encode(JsonSerializer.Serialize(scoreBoard));
            File.WriteAllText(path,data);
        }

        public string GetFilePath(bool isFile)
        {
            string path;
            if (isFile)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                          + $@"\RHServerDB\UNO\Scoreboard.json";
             
            }
            else
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                              + $@"\RHServerDB\UNO";
            }
            return path;
        }

        public List<Score> ReadFile()
        {
            string Dirpath = GetFilePath(false);
            if (!Directory.Exists(Dirpath))
            {
                Directory.CreateDirectory(Dirpath);
            }

            string Filepath = GetFilePath(true);
            string data = Decode(File.ReadAllText(Filepath));

            JObject o1 = JObject.Parse(data);
            return o1.ToObject<List<Score>>();
        }

        public Score getScoreByUser(string Username)
        {
            foreach (Score score in scoreBoard)
            {
                if (score.username == Username)
                {
                    return score;
                }
            }
            Score addedScore =  new Score(Username, 0,0);
            scoreBoard.Add(addedScore);
            return addedScore;
        }

        public void updateScore(Score Upadatescore)
        {
            foreach (Score score in scoreBoard)
            {
                if (score.username == Upadatescore.username)
                {
                    score.winAmount = Upadatescore.winAmount;
                    score.gameAmount = Upadatescore.gameAmount;
                }
            }
        }

    }
}
