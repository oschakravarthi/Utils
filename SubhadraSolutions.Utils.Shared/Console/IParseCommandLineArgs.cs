namespace SubhadraSolutions.Utils.Console;

public interface IParseCommandLineArgs
{
    bool WasInvokedByCommandLine { get; set; }

    IActor GetActor();

    string GetHelpDetails();

    string GetHelpExamples();

    string GetHelpTrigger();

    string GetHelpUsage();

    bool ParseCommandLineArgs(string[] args, ref int index);

    bool? WereRequiredArgsProvided();
}