// See https://aka.ms/new-console-template for more information
using CommandLine;

public class Args
{
    [Option('d', "decrypt", Required = false, HelpText = "Decrypt the input file.")]
    public bool Decrypt { get; set; }

    [Option('i', "input", Required = true, HelpText = "Input file to read.")]
    public string Input { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file to write.")]
    public string Output { get; set; }
}