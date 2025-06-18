using System.Diagnostics;
using Vosk;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// Serve wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/recognize", async (HttpRequest req) =>
{
    using var reader = new StreamReader(req.Body);
    var base64Audio = await reader.ReadToEndAsync();

    // Create unique file names in tmp folder to avoid conflicts
    var uuid = Guid.NewGuid().ToString();
    var tmpPath = Path.GetTempPath();
    var inputFile = Path.Combine(tmpPath, uuid + ".ogg");
    var wavFile = Path.Combine(tmpPath, uuid + ".wav");

    // Write base64 input stream audio to file
    await File.WriteAllBytesAsync(inputFile, Convert.FromBase64String(base64Audio));

    // Convert ogg to wav
    var ffmpegArgs = $"-y -i {inputFile} -ar 16000 -ac 1 -f wav {wavFile}";
    var ffmpeg = Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = ffmpegArgs,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    });

    // Wait for ffmpeg to finish
    await ffmpeg.WaitForExitAsync();

    // Read model path from config
    var modelPath = builder.Configuration["VoskModelPath"];

    // Initialize Vosk
    Vosk.Vosk.SetLogLevel(0);
    using var model = new Model(modelPath);
    using var rec = new VoskRecognizer(model, 16000.0f);

    // Read wav file
    var waveData = File.ReadAllBytes(wavFile);

    // Recognize
    rec.AcceptWaveform(waveData, waveData.Length);
    var resultJson = rec.Result();

    // Format result
    var recognizedText = System.Text.Json.JsonDocument.Parse(resultJson).RootElement.GetProperty("text").GetString();

    // Cleanup temporary files
    File.Delete(inputFile);
    File.Delete(wavFile);

    return Results.Ok(recognizedText ?? "");
});

app.Run();
