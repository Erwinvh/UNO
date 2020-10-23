using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static SharedDataClasses.Encryption;

namespace Server
{
    class FileSystem
    {
        public Scoreboard scoreBoard { get; set; }

        public FileSystem()
        {
            ReadFile();
            //scoreBoard = new Scoreboard(new List<Score>());
            //scoreBoard.scoreboard.Add(new Score("tester", 10,20));
            //WritetoFile();
        }

        public void WritetoFile()
        {
            directoryExists();
            string path = GetFilePath(true);
            string data = JsonSerializer.Serialize(scoreBoard);
            File.WriteAllText(path,data);
        }

        public void directoryExists()
        {
            string Dirpath = GetFilePath(false);
            if (!Directory.Exists(Dirpath))
            {
                Directory.CreateDirectory(Dirpath);
            }
        }

        public string GetFilePath(bool isFile)
        {
            string path;
            if (isFile)
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                          + $@"\CUNOServerDB\UNO\Scoreboard.json";
             
            }
            else
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                              + $@"\CUNOServerDB\UNO";
            }
            return path;
        }

        public void ReadFile()
        {
            directoryExists();
            string Filepath = GetFilePath(true);
            string data = File.ReadAllText(Filepath);

            JObject o1 = JObject.Parse(data);
            scoreBoard = o1.ToObject<Scoreboard>();
            
        }

        public Score getScoreByUser(string Username)
        {
            foreach (Score score in scoreBoard.scoreboard)
            {
                if (score.username == Username)
                {
                    return score;
                }
            }
            Score addedScore =  new Score(Username, 0,0);
            scoreBoard.scoreboard.Add(addedScore);
            return addedScore;
        }

        public void updateScore(Score Upadatescore)
        {
            foreach (Score score in scoreBoard.scoreboard)
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
