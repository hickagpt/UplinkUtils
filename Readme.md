# Redshirt .NET Tool

Redshirt is a .NET tool for encrypting and decrypting files using a custom algorithm. This tool is built using .NET 8 and C# 12.0.

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or any other compatible IDE

## Installation

1. Clone the repository:

```
git clone https://github.com/your-repo/redshirt.git
cd redshirt
```

2. Build the solution:

```
dotnet build
```

## Usage

The Redshirt tool can be used via the command line. It supports both encryption and decryption of files.

### Command Line Arguments

- `-i, --input` (required): The input file path.
- `-o, --output` (required): The output file path.
- `-d, --decrypt` (optional): Flag to indicate decryption. If not provided, the tool will encrypt the file.

### Examples

#### Encrypt a File

To encrypt a file, use the following command:

```
dotnet run --project Uplink.Redshirt.Cli -- -i path/to/input/file -o path/to/output/file
```

#### Decrypt a File

To decrypt a file, use the following command:

```
dotnet run --project Uplink.Redshirt.Cli -- -i path/to/input/file -o path/to/output/file -d
```

## Project Structure

- `Uplink.Redshirt`: Contains the core encryption and decryption logic.
- `Uplink.Redshirt.Cli`: Command line interface for the Redshirt tool.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.