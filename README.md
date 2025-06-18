# VoiceRecognitionDemo
This is a minimal proof-of-concept (PoC) project using .NET 8 Minimal API and the Vosk speech recognition engine for offline voice-to-text conversion. It includes:

A backend API built with .NET 8

Static HTML/JS frontend for audio recording and display

Integration with Vosk via NuGet

## Requirements

.NET 8 SDK

Visual Studio or Visual Studio Code

FFmpeg installed and available in your system PATH (used to convert OGG to WAV)

## Setup

Install dependencies:

The project uses the Vosk NuGet package. When you build, it will be restored automatically.

Download a Vosk model:

Download an English model from Vosk's website and extract it to the following location:

<project-root>/models/<your-model>

For example:

models/vosk-model-small-en-us-0.15/

Ensure model copying:

The project is configured to copy the models/ folder to the build output (bin/...) directory. The API expects this folder to exist because the path is set in appsettings.json like this:

"VoskModelPath" : "models/vosk-model-small-es-0.42"

If you change the model name or path, update appsettings.json accordingly.

## Running

Run the project from Visual Studio or using the CLI:

dotnet run

Then open your browser at http://localhost:5000 (or the port shown in console).

## Notes

Works fully offline.

Frontend is pure static HTML and JavaScript.

Tested with modern browsers supporting MediaRecorder.

Requires FFmpeg for converting audio to WAV format.