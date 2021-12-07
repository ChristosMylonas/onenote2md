using Onenote2md.Core;
using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Cmd
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var notebookName = "";
                var outputDirectory = Environment.CurrentDirectory;

                switch (args.Length)
                {
                    case 1:
                        {
                            notebookName = args[0];
                        }
                        break;

                    case 2:
                        {
                            notebookName = args[0];
                            outputDirectory = args[1];
                        }
                        break;

                    default:
                        ShowHelp();
                        return -1;
                }

                var oneNoteApp = OneNoteApplication.Instance;
                var notebook = oneNoteApp.GetNotebook(notebookName);
                if (notebook == null)
                {
                    Console.WriteLine("Could not find Notebook:{0}", notebookName);
                    return -2;
                }

                INotebookGenerator notebookParser = new NotebookParser(oneNoteApp, new MDGenerator(oneNoteApp));
                var writer = new MDWriter(outputDirectory, true);
                notebookParser.GenerateNotebookMD(notebook, writer);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: {0}", ex);
                return -3;
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("An automated tool to convert OneNote notebooks to markdown files.");
            Console.WriteLine();
            Console.WriteLine("Onenote2md.Cmd NOTEBOOK_NAME [OUTPUT_DIRECTORY]");
            Console.WriteLine();
            Console.WriteLine("  NOTEBOOK_NAME is your notebook name (case-insensitive).");
            Console.WriteLine("  [OUTPUT_DIRECTORY] (optionally) is requested output directory.");
        }
    }
}
