using CommandLine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using Figgle;
using System.IO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System;
using Size = SixLabors.ImageSharp.Size;
using Image = SixLabors.ImageSharp.Image;
using XUCore.ShellProgressBar;
using Ultra.Framework.Strings.Helpers;

namespace Ultra.BulkImageProcessor;

static class Program
{
    // Define a class to hold command-line options
    public class Options
    {
        [Option('s', "source", Required = false, HelpText = "The source path of images.")]
        public string SourcePath { get; set; } = "";

        [Option('o', "output", Required = false, HelpText = "The output path to save resized images.")]
        public string OutputPath { get; set; } = "";

        [Option('w', "width", Required = false, HelpText = "The maximum width to resize images to.")]
        public int? Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "The maximum height to resize images to.")]
        public int? Height { get; set; }

        [Option('r', "rmode", Required = false, Default = "Pad", HelpText = "Resize mode (Pad, BoxPad, Crop, Max, etc.).")]
        public string ResizeMode { get; set; } = "Pad";

        [Option('q', "quality", Required = false, Default = 75, HelpText = "The quality of the output images (1-100). Applies to JPEG format.")]
        public int Quality { get; set; } = 75;

        [Option('f', "format", Required = false, HelpText = "The format to convert images to (leave empty to keep original format). e.g., jpg, png.")]
        public string OutputFormat { get; set; } = "";  // New option for format change
    }

    static async Task Main(string[] args)
    {
        // Parse command-line arguments
        var parserResult = Parser.Default.ParseArguments<Options>(args);
        await parserResult.WithParsedAsync(async options =>
        {
            // Prompt user for missing options
            InteractivePromptIfMissing(options);

            // Process images after gathering all required information
            await ProcessImagesAsync(options.SourcePath, options.OutputPath, options.Width, options.Height, options.ResizeMode, options.Quality, options.OutputFormat);
        });
    }

    static void InteractivePromptIfMissing(Options options)
    {
        // If source path is missing, prompt the user for it
        if (string.IsNullOrWhiteSpace(options.SourcePath))
        {
            Console.Write("Enter the source path of images: ");
            options.SourcePath = Console.ReadLine()?.Trim() ?? "";
        }

        // If output path is not provided, set the default output path or ask the user
        if (string.IsNullOrWhiteSpace(options.OutputPath))
        {
            Console.Write("Enter the output path to save resized images (or press Enter to use default): ");
            var userOutputPath = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(userOutputPath))
            {
                options.OutputPath = Path.Combine(options.SourcePath, "compressed-result");
                Directory.CreateDirectory(options.OutputPath);
            }
            else
            {
                options.OutputPath = userOutputPath;
            }
        }

        // Prompt for width if missing
        if (!options.Width.HasValue)
        {
            Console.Write("Enter the maximum width for the resized images (or press Enter to skip): ");
            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int width))
            {
                options.Width = width;
            }
        }

        // Prompt for height if missing
        if (!options.Height.HasValue)
        {
            Console.Write("Enter the maximum height for the resized images (or press Enter to skip): ");
            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int height))
            {
                options.Height = height;
            }
        }

        // Prompt for resize mode if not provided
        if (string.IsNullOrWhiteSpace(options.ResizeMode))
        {
            Console.Write("Enter the resize mode (Pad, BoxPad, Crop, Max, etc.) (default: Pad): ");
            var input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                options.ResizeMode = input;
            }
        }

        // Prompt for quality if not provided
        if (options.Quality <= 0 || options.Quality > 100)
        {
            Console.Write("Enter the quality of the output images (1-100, default: 75): ");
            var input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int quality) && quality > 0 && quality <= 100)
            {
                options.Quality = quality;
            }
        }

        // Prompt for output format if not provided
        if (string.IsNullOrWhiteSpace(options.OutputFormat))
        {
            Console.Write("Enter the format to convert images to (jpg, png, etc., or press Enter to keep original): ");
            options.OutputFormat = Console.ReadLine()?.Trim() ?? "";
        }
    }

    static async Task ProcessImagesAsync(string sourcePath, string outputPath, int? width, int? height, string resizeMode, int quality, string outputFormat)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            outputPath = Path.Combine(sourcePath, "compressed-result");
            Directory.CreateDirectory(outputPath);
        }

        // Get all image files recursively
        var imageFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)
                                  .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".jpeg"))
                                  .ToList();

        if (imageFiles.Count == 0)
        {
            Console.WriteLine("No images found in the source path.");
            return;
        }

        var progressBarOptions = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Cyan,
            BackgroundColor = ConsoleColor.DarkGray,
            ProgressCharacter = '─',
            CollapseWhenFinished = true
        };

        using (var progressBar = new ProgressBar(imageFiles.Count, "Resizing images...", progressBarOptions))
        {
            var tasks = imageFiles.Select(async (imageFile, _) =>
            {
                // Create the corresponding subdirectory structure in the output path
                string relativePath = Path.GetRelativePath(sourcePath, Path.GetDirectoryName(imageFile)!);
                string newOutputDir = Path.Combine(outputPath, relativePath);
                Directory.CreateDirectory(newOutputDir);

                await ProcessSingleImageAsync(imageFile, newOutputDir, width, height, resizeMode, quality, outputFormat);
                progressBar.Tick($"Processed {Path.GetFileName(imageFile)}");
            });

            await Task.WhenAll(tasks);
        }

        Console.WriteLine(FiggleFonts.Standard.Render("Compression Completed!"));
    }

    static async Task ProcessSingleImageAsync(string imagePath, string outputPath, int? width, int? height, string resizeMode, int quality, string outputFormat)
    {
        using Image image = await Image.LoadAsync(imagePath);
        // Check if image needs resizing based on user-defined dimensions
        bool shouldResize = (width.HasValue && image.Width > width.Value) || (height.HasValue && image.Height > height.Value);

        if (shouldResize)
        {
            var options = GetResizeOptions(width, height, resizeMode);
            image.Mutate(x => x.Resize(options));
        }

        // Determine output format and file extension
        var extension = string.IsNullOrWhiteSpace(outputFormat) ? Path.GetExtension(imagePath).ToLower() : $".{outputFormat.ToLower()}";

        // Construct output file path and ensure valid file name
        var outputFilePath = Path.Combine(outputPath, StringHelpers.CleanFileNameString(Path.GetFileNameWithoutExtension(imagePath)) + extension);

        // Save image with appropriate format and quality settings
        if (extension == ".jpg" || extension == ".jpeg")
        {
            var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = quality };
            await image.SaveAsync(outputFilePath, encoder);
        }
        else if (extension == ".png")
        {
            var encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder();
            await image.SaveAsync(outputFilePath, encoder);
        }
        else if (extension == ".webp")
        {
            var encoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
            {
                Quality = quality // WebP quality as integer (1 to 100)
            };
            await image.SaveAsync(outputFilePath, encoder);
        }
        #region avif-support
        //else if (extension == ".avif")
        //{
        //    // Use AVIF encoder for .avif output format
        //    var encoder = new SixLabors.ImageSharp.Formats.Avif.AvifEncoder
        //    {
        //        Quality = quality // AVIF quality as integer (1 to 100)
        //    };
        //    await image.SaveAsync(outputFilePath, encoder);
        //}
        #endregion
        else
        {
            // If no format change is specified, save in the original format
            await image.SaveAsync(outputFilePath);
        }
    }



    static ResizeOptions GetResizeOptions(int? width, int? height, string resizeMode)
    {
        return new ResizeOptions
        {
            Mode = Enum.TryParse<ResizeMode>(resizeMode, true, out var mode) ? mode : ResizeMode.Pad,
            Size = new Size(width ?? 0, height ?? 0)
        };
    }
}
