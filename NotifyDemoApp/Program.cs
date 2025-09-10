using WindowsToastNotifyApi;

class Program
{
    [STAThread]
    static void Main()
    {
        Toast.Initialize("Ben.WindowsToastNotifyApi.Demo", "Toast Demo", null);

        Toast.Activated += a =>
        {
            Console.WriteLine($"Activated: {a.Arguments}");
            foreach (var kv in a.Payload)
                Console.WriteLine($"{kv.Key}={kv.Value}");
        };

        Toast.Info("Build finished", "Your NuGet package was published.");
        Toast.Success("Backup complete", "All files synced.");
        Toast.Warning("High CPU", "DeepSeek is using 92% CPU.");
        Toast.Error("Service crashed", "Watcher stopped unexpectedly.");

        Toast.Show("Deploy ready", "Promote to production?",
            new ToastOptions
            {
                PrimaryButton = ("Deploy now", "deploy"),
                SecondaryButton = ("Later", "later"),
                Payload = new Dictionary<string, string> { ["pipelineId"] = "a1b2c3" },
                Duration = ToastDuration.Long
            });

        Console.ReadKey(); // gewünscht
    }
}
