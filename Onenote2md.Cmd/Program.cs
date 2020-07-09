using Onenote2md.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var notebookName = "";
            var outputDirectory = Environment.CurrentDirectory;

            switch (args.Count())
            {
                case 0:
                    ShowHelp();
                    return;

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
                    return;
            }


            var notebookParser = new NotebookParser();
            var writer = new MDWriter(outputDirectory, true);
            var generator = new MDGenerator(notebookParser);
            generator.GenerateNotebookMD(notebookName, writer);
        }

        static void ShowHelp()
        {
            Console.WriteLine("An automated tool to convert OneNote notebooks to markdown files.");
            Console.WriteLine();
            Console.WriteLine("Onenote2md.Cmd NOTEBOOK_NAME [OUTPUT_DIRECTORY]");
            Console.WriteLine();
            Console.WriteLine("  NOTEBOOK_NAME is your notebook name.");
            Console.WriteLine("  [OUTPUT_DIRECTORY] (optionally) is requested output directory.");
        }
    }
}
