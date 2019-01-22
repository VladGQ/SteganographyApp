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
                InputArguments inputArgs = new ArgumentParser().Parse(args);

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
            Console.WriteLine("SteganographyApp Help");

            Console.WriteLine("Arguments must be specified as <argument_name>space<argument_value>.");
            Console.WriteLine("\tExample arguments for encoding a file to a set of images: ");
            Console.WriteLine("\t\tdotnet .\\SteganographApp =-action encode --images 001.png,002.png --input FileToEncode.zip --password Pass1234 --randomSeed monkey --compress");
            Console.WriteLine("\tExample arguments for decoding data from a set of images to an output file.");
            Console.WriteLine("\t\tdotnet .\\SteganpgraphyApp --action decode --images 001.png,002.png --output DecodedOutputFile.zip --password Pass1234 --randomSeed monkey --compress\n");

            Console.WriteLine("\t--action | -a :: Specifies whether to 'encode' a file to a set of images or 'decode' a set of images to a file.");
            Console.WriteLine("\t\tValue must be either 'encode', 'decode', or 'clean'.");
            Console.WriteLine("\t\tClean specifies that all LSBs in the set of images will be overwritten with garbage values.");

            Console.WriteLine("\t--input | -in :: The path to the file to encode if 'encode' was specified in the action argument.");

            Console.WriteLine("\t--output | -o :: The path to the output file when 'decode' was specified in the action argument.");

            Console.WriteLine("\t--images | -im :: A comma delimited list of paths to images to be either encoded or decoded");
            Console.WriteLine("\t\tThe order of the images affects the encoding and decoding results.");
            Console.WriteLine("\t\tThis parameter will also accept a regular expression to fine images.");
            Console.WriteLine("\t\t\tA regex value will appear in the format [r]<regex><directory>");
            Console.WriteLine("\t\t\tExample: --images [r]<^.*\\.(png)><.> looks for all png files in the current directory.");

            Console.WriteLine("\t--passsword | -p :: The password to encrypt the input file when 'encode' was specified in the action argument.");
            Console.WriteLine("\t\tEnter ? for the password to input the real password in interactive mode.");

            Console.WriteLine("\t--printStack | -stack :: Specifies whether or not to print the full stack trace if an error occurs.");
            Console.WriteLine("\t\tNo value is required for this argument. When provided it will always be true otherwise it will be false.");

            Console.WriteLine("\t--compress | -c :: Specifies whether or not to compress/decompress the encoded/decoded content.");
            Console.WriteLine("\t\tNo value is required for this argument. When provided it will always be true otherwise it will be false.");

            Console.WriteLine("\t--chunkSize | -cs :: Specifies the number of bytes to read in each read, encode, and store operation.");
            Console.WriteLine("\t\tValue needs to be a positive whole number.");
            Console.WriteLine("\t\tLarger numbers can quicken the process of encoding larger files and make the encoded file size smaller.");
            Console.WriteLine("\t\tValues that are too large can produce out of memory errors.");

            Console.WriteLine("\t--randomSeed | -rs :: Randomizes the order in which bytes will be written from input file to image.");
            Console.WriteLine("\t\tEnter ? for the randomSeed to input the real randomSeed in interactive mode.");

            Console.WriteLine();
        }
    }
}