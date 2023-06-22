using Newtonsoft.Json;

namespace ZDrive.Services;

public class ConfigProvider
{
    private readonly Dictionary<string, object> configs;

    public ConfigProvider(string configFile)
    {
        StreamReader SR = new StreamReader(configFile);
        string result = "";
        result = SR.ReadToEnd();

        configs = JsonConvert.DeserializeObject<Dictionary<string, object>>(result) ?? null!;
        if (configs == null) throw new FileLoadException();
    }

    public string this[string key]
        => configs[key].ToString() ?? "";
}
