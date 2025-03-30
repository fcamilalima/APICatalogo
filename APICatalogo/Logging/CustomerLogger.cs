
namespace APICatalogo.Logging;

public class CustomerLogger : ILogger
{
    readonly string _loggerName;
    readonly CustomLoggerProviderConfiguration _loggerConfig;
    public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
    {
        _loggerName = name;
        _loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == _loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";
        EscreverTextoNoArquivo(mensagem);
    }

    private void EscreverTextoNoArquivo(string mensagem)
    {
        string diretorioLog = @"D:\Dados\Log\";
        string arquivoLog = Path.Combine(diretorioLog, "APICatalogo.txt");

        if (!Directory.Exists(diretorioLog))
        {
            Directory.CreateDirectory(diretorioLog);
        }

        if (!File.Exists(arquivoLog))
        {
            using (File.Create(arquivoLog)) { }
        }

        using (StreamWriter streamWriter = new StreamWriter(arquivoLog, true))
        {
            try
            {
                streamWriter.WriteLine(mensagem);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro: {ex.Message}");
            }
        }
    }
}
