using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using SharedDataClasses;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static SharedDataClasses.Encryption;

namespace Server
{
    public class FileSystem
    {
        public Scoreboard scoreBoard { get; set; }

        public FileSystem()
        {
            ReadFile();
        }

        //
        //--Writes scoreboard to local file--
        //
        public void WritetoFile()
        {
            directoryExists();
            string path = GetFilePath(true);
            string data = JsonSerializer.Serialize(scoreBoard);
            File.WriteAllText(path,data);
        }

        //
        //--Checks if directory exists, if not it creates the desired directory.--
        //
        public void directoryExists()
        {
            string Dirpath = GetFilePath(false);
            if (!Directory.Exists(Dirpath))
            {
                Directory.CreateDirectory(Dirpath);
            }
            FileExists();
        }

        //
        //--Checks if file already exists, if not it writes to a new file--
        //
        public void FileExists()
        {
            string filepath = GetFilePath(true);
            if (!File.Exists(filepath))
            {
                File.WriteAllText(filepath, JsonSerializer.Serialize(new Scoreboard(new List<Score>())));
            }
        }

        //
        //--Gets the filepath for writing players scoreboard--
        //
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

        //
        //--Reads the local scoreboard file--
        //
        public void ReadFile()
        {
            directoryExists();
            string Filepath = GetFilePath(true);
            string data = File.ReadAllText(Filepath);

            JObject o1 = JObject.Parse(data);
            scoreBoard = o1.ToObject<Scoreboard>();
            
        }

        //
        //--Returns the score for one user only.--
        //
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

        //
        //--Updates the scoreboard--
        //
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
