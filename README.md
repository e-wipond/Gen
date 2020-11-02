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

```python
gen .\src .\dist
```

The generated site will be output to the `dist` directory. 

See the `docs-src` directory for examples on use.

## License

[MIT](https://choosealicense.com/licenses/mit/)