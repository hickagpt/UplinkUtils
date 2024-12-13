// See https://aka.ms/new-console-template for more information
using CommandLine;
using Uplink.Redshirt;

Console.WriteLine("Redshirt .NET");

Parser.Default.ParseArguments<Args>(args)
    .WithParsed<Args>(opts =>
    {
        var redshirt = new Redshirt();

        if (!File.Exists(opts.Input))
        {
            Console.WriteLine($"Input file {opts.Input} does not exist.");
            return;
        }

        try
        {
            var inputBytes = File.ReadAllBytes(opts.Input);

            byte[] result;

            if (opts.Decrypt)
            {
                result = redshirt.Decrypt(inputBytes);
            }
            else
            {
                result = redshirt.Encrypt(inputBytes);
            }

            File.WriteAllBytes(opts.Output, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    });