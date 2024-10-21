# Ultra Bulk Image Processor

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

The **Ultra Bulk Image Processor** is the fastest utility to resize and compress your image files in bulk for web uploads and other uses. Designed to be powerful yet simple, it streamlines the process of preparing images for web use, saving you valuable time and effort.

## Table of Contents

- [Features](#features)
- [Vision and Goal](#vision-and-goal)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
  - [Command-Line Options](#command-line-options)
  - [Examples](#examples)
  - [Supported Resize Modes](#supported-resize-modes)
- [Roadmap](#roadmap)
- [License](#license)
- [Contributing](#contributing)
- [Acknowledgments](#acknowledgments)

---

## Features

- **Bulk Processing**: Process individual images or entire directories recursively.
- **Flexible Resizing**: Specify maximum width and height, with various resize modes.
- **Format Conversion**: Convert images to formats like JPG, PNG, or keep the original.
- **Quality Control**: Adjust output quality for JPEG images (1-100 scale).
- **Interactive Prompts**: Automatically prompts for missing information.
- **Progress Tracking**: Visual progress bar for bulk operations.
- **Error Handling**: Gracefully handles missing files or unsupported formats.

## Vision and Goal

Our vision is to provide the fastest and most efficient utility for resizing and compressing images in bulk, tailored for web uploads and other high-volume needs. We aim to simplify the workflow for developers, designers, and content creators by offering a tool that's both powerful and easy to use.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/aloksharma1/Ultra.BulkImageProcessor.git
   ```

2. **Navigate to the project directory**

   ```bash
   cd UltraBulkImageProcessor
   ```

3. **Build the project**

   ```bash
   dotnet build
   ```

   Alternatively, publish it as a standalone executable:

   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true
   ```

## Usage

You can run the utility directly from the command line. It accepts various options to customize the image processing according to your needs.

### Command-Line Options

```plaintext
-s, --source      The source path of images.

-o, --output      The output path to save resized images.

-w, --width       The maximum width to resize images to.

-h, --height      The maximum height to resize images to.

-r, --rmode       Resize mode (Pad, BoxPad, Crop, Max, etc.). Default is Pad.

-q, --quality     The quality of the output images (1-100). Applies to JPEG format. Default is 75.

-f, --format      The format to convert images to (leave empty to keep original format). e.g., jpg, png.

--help            Display this help screen.

--version         Display version information.
```

### Examples

#### Example 1: Basic Usage

Resize all images in a directory to a maximum width of 800 pixels, keeping the original format.

```bash
dotnet run -- -s "C:\Images\Source" -o "C:\Images\Output" -w 800
```

#### Example 2: Specifying Height and Format Conversion

Resize images to a maximum width of 800 pixels and height of 600 pixels, converting all images to JPEG format with 80% quality.

```bash
dotnet run -- -s "C:\Images\Source" -o "C:\Images\Output" -w 800 -h 600 -f jpg -q 80
```

#### Example 3: Using a Different Resize Mode

Resize images using the `Crop` resize mode.

```bash
dotnet run -- -s "C:\Images\Source" -w 800 -r Crop
```

#### Example 4: Processing a Single Image

Process a single image file.

```bash
dotnet run -- -s "C:\Images\Source\image.jpg" -o "C:\Images\Output" -w 800 -h 600
```

#### Example 5: Interactive Mode

Run the utility without any options to enter interactive mode. You will be prompted for each required input.

```bash
dotnet run
```

Output:

```plaintext
Enter the source path of images: C:\Images\Source
Enter the output path to save resized images (or press Enter to use default):
Enter the maximum width for the resized images (or press Enter to skip): 800
Enter the maximum height for the resized images (or press Enter to skip): 600
Enter the resize mode (Pad, BoxPad, Crop, Max, etc.) (default: Pad):
Enter the quality of the output images (1-100, default: 75):
Enter the format to convert images to (jpg, png, etc., or press Enter to keep original):
```

### Supported Resize Modes

- **Pad**: Pads the image to fit the specified dimensions without distorting the aspect ratio.
- **BoxPad**: Similar to Pad but ensures the image fits within the specified box dimensions.
- **Crop**: Crops the image to fill the specified dimensions.
- **Max**: Resizes the image to fit within the specified dimensions while maintaining aspect ratio.
- **Min**: Ensures the image dimensions are at least the specified size.
- **Stretch**: Stretches the image to exactly match the specified dimensions (may distort aspect ratio).

## Roadmap

We are continuously improving the Ultra Bulk Image Processor. Here are some of the features we plan to add:

- **AVIF Format Support**: Implement support for the AVIF image format.
- **Metadata Handling**: Options to preserve or remove image metadata (EXIF data).
- **Watermarking**: Ability to add custom watermarks to images during processing.
- **Batch Processing Enhancements**: Multi-threading for faster processing on multi-core systems.
- **GUI Application**: Develop a graphical user interface for users who prefer not to use the command line.
- **Extended Format Support**: Add support for additional image formats like TIFF and BMP.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

We welcome contributions! Please read our [Contributing Guidelines](CONTRIBUTING.md) for details on how to get started.

## Acknowledgments

- **[CommandLineParser](https://github.com/commandlineparser/commandline)**: For handling command-line arguments.
- **[SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)**: For image processing capabilities.
- **[Figgle](https://github.com/drewnoakes/figgle)**: For generating ASCII art in the console.
- **[XUCore.ShellProgressBar](https://github.com/xuyuanxiang/ShellProgressBar)**: For the interactive progress bar.
- **[Ultra.Framework Libraries](https://github.com/aloksharma1/Ultra.Framework)**: For helper methods used in the project.

---

Feel free to reach out if you have any questions or need assistance using the Ultra Bulk Image Processor. We're here to help!