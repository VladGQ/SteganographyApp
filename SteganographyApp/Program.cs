﻿using SteganographyApp.Common;
using SteganographyApp.Common.Data;
using System;

namespace SteganographyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nSteganography App\n");
            if (Array.IndexOf(args, "--help") != -1)
            {
                PrintHelp();
                return;
            }

            try
            {
                InputArguments inputArgs = ArgumentParser.Instance.Parse(args);

                try
                {
                    new EntryPoint(inputArgs).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured during execution: ");
                    Console.WriteLine("\tException Message: {0}", e.Message);
                    if (inputArgs.PrintStack)
                    {
                        Console.WriteLine(e.StackTrace);
                    }

                    switch(e)
                    {
                        case TransformationException t:
                            Console.WriteLine("This error often occurs as a result of an incorrect password when decrypting a file.");
                            break;
                    }
                }
            }
            catch (ArgumentParseException e)
            {
                Console.WriteLine("\nAn exception occured while parsing arguments:\n\t{0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("And was caused by:\n\t{0}", e.InnerException.Message);
                }
                Console.WriteLine("\nRun the program with --help to get more information.");
            }

            Console.WriteLine("");
        }

        static void PrintHelp()
        {
            Console.WriteLine("Help");
            Console.WriteLine("Arguments must be specified with and = sign and no spaces.");
            Console.WriteLine("\tExample arguments for encoding a file to a set of images: ");
            Console.WriteLine("\t\tdotnet .\\SteganographApp =-action=encode --images=001.png,002.png --input=FileToEncode.zip --password=Pass1234 --randomSeed=monkey --compress=true");
            Console.WriteLine("\tExample arguments for decoding data from a set of images to an output file.");
            Console.WriteLine("\t\tdotnet .\\SteganpgraphyApp --action=decode --images=001.png,002.png --output=DecodedOutputFile.zip --password=Pass1234 --randomSeed=monkey --compress=true\n");
            Console.WriteLine("\t--action :: Specifies whether to 'encode' a file to a set of images or 'decode' a set of images to a file.");
            Console.WriteLine("\t\tValue must be either 'encode', 'decode', or 'clean'.");
            Console.WriteLine("\t\tClean specifies that all LSBs in the set of images will be overwritten with garbage values.");
            Console.WriteLine("\t--input :: The path to the file to encode if 'encode' was specified in the action argument.");
            Console.WriteLine("\t--output :: The path to the output file when 'decode' was specified in the action argument.");
            Console.WriteLine("\t--images :: A comma delimited list of paths to images to be either encoded or decoded");
            Console.WriteLine("\t\tThe order of the images affects the encoding and decoding results.");
            Console.WriteLine("\t\tThis parameter will also accept a regular expression to fine images.");
            Console.WriteLine("\t\t\tA regex value will appear in the format [r]<regex><directory>");
            Console.WriteLine("\t\t\tExample: --images=[r]<^.*\\.(png)><.> looks for all png files in the current directory.");
            Console.WriteLine("\t--passsword :: The password to encrypt the input file when 'encode' was specified in the action argument.");
            Console.WriteLine("\t--printStack :: Specifies whether or not to print the full stack trace if an error occurs.");
            Console.WriteLine("\t\tValue must either be 'true' or 'false'");
            Console.WriteLine("\t--compress :: Specifies whether or not to compress/decompress the encoded/decoded content.");
            Console.WriteLine("\t\tValue must be either 'true' or 'false'.");
            Console.WriteLine("\t--chunkSize :: Specifies the number of bytes to read in each read, encode, and store operation.");
            Console.WriteLine("\t\tValue needs to be a positive whole number.");
            Console.WriteLine("\t\tLarger numbers can quicken the process of encoding larger files.");
            Console.WriteLine("\t--randomSeed :: Randomizes the order in which bytes will be written from input file to image.");
            Console.WriteLine();
        }
    }
}