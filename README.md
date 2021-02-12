# Gen

A tiny C# .NET Core static site generator. 

[See it in action](https://e-wipond.github.io/Gen/).

## Features

- Templating and partials
- Variable replacement 
- Breadcrumb navigation 
- Next/Previous navigation 
- Markdown support
- Syntax highlighting (Scheme)
- Blog post support 

## Installation

Install Gen as a [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install).

```bash
dotnet tool install --global Gen --version 1.0.0-alpha.1
```

## Usage

Create a `src` directory containing HTML and markdown files and run

```bash
gen .\src .\dist
```

The generated site will be output to the `dist` directory. 

See the `docs-src` directory for examples on use.

## Package

Run

```bash
dotnet pack .\Gen\
```

and find the .nupkg file in the `\Gen\GenCli\nupkg\` folder.

## License

[MIT](https://choosealicense.com/licenses/mit/)

## To do

- add support for inlining code files
- logging
- dependency injection 
- performance tests
- parallelize