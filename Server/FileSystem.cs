using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Server
{
    class FileSystem
    {
        public FileSystem()
        {

        }

        public void WritetoFile()
        {

        }

        public string GetFilePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                          + $@"\RHServerDB\UNO\";
            return null;
        }

        public void ReadFile()
        {

        }

    }
}
