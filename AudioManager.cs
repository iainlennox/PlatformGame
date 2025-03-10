using System.Runtime.InteropServices;
using System.Threading.Tasks;
#if WINDOWS
using System.Speech.Synthesis;
#endif

namespace PlatformGame;

public class AudioManager
{
#if WINDOWS
    private static SpeechSynthesizer synthesizer;
#endif

    public static void Initialize()
    {
#if WINDOWS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                synthesizer.Rate = 0;
                synthesizer.Volume = 100;
            }
            catch
            {
                // Fail silently if speech synthesis is not available
                synthesizer = null;
            }
        }
#endif
    }

    public static async Task SpeakAsync(string text)
    {
#if WINDOWS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && synthesizer != null)
        {
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                await Task.Run(() => synthesizer.Speak(text));
#pragma warning restore CA1416
            }
            catch
            {
                // Fail silently if speech fails
            }
        }
#endif
    }

    public static void Cleanup()
    {
#if WINDOWS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && synthesizer != null)
        {
            synthesizer.Dispose();
            synthesizer = null;
        }
#endif
    }
}