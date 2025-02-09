using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubhadraSolutions.Utils.Console;

public class CommandLineParserBase
{
    public static string DerivedClassPreamble = string.Empty;

    public static bool isDebugEnabled;

    public static Verbosity verbosity = Verbosity.Default;

    internal static HandlerListComparer Comparer = new();

    protected static Dictionary<string, IParseCommandLineArgs> _otherParsers = [];

    protected static List<Action> _partialInitializers = [];

    protected static Dictionary<Action, bool> _partialInitializersExecuted = [];

    public bool WasInvokedByCommandLine;

    public static bool AddInitializer(Action partialInitializer)
    {
        _partialInitializers.ThreadSafeWrite(delegate
        {
            _partialInitializers.Add(partialInitializer);
            _partialInitializersExecuted.Add(partialInitializer, false);
        });
        return true;
    }

    public static bool AddParser(string argName, IParseCommandLineArgs parserClass)
    {
        if (argName[0] == '-' || argName[0] == '/')
        {
            argName = argName.Substring(1);
        }

        argName = "/" + argName;
        _otherParsers.ThreadSafeWrite(delegate { _otherParsers.Add(argName, parserClass); });
        return true;
    }

    public static List<IActor> GetChosenActors()
    {
        return (from item in _otherParsers.Distinct(Comparer)
                where item.Value.WasInvokedByCommandLine
                select item.Value.GetActor()).ToList();
    }

    public static bool Parse(string[] args)
    {
        var flag = true;
        Init();
        if (args.Length == 0)
        {
            Usage();
            flag = false;
        }
        else
        {
            var num = -1;
            for (var i = 0; i < args.Length; i++)
                if (args[i] == "--" || args[i] == "#")
                {
                    num = i;
                    break;
                }

            if (num >= 0)
            {
                args = args.Take(num).ToArray();
            }

            args = args.Select(arg =>
            {
                if (arg[0] == '-')
                {
                    arg = "/" + arg.Substring(1);
                }

                return arg;
            }).ToArray();
            var ii = 0;
            while (true)
            {
                if (ii < args.Length)
                {
                    var argName = GetArgName(args, ref ii);
                    switch (argName)
                    {
                        case "/?":
                        case "/h":
                        case "/help":
                            Usage();
                            flag = false;
                            break;

                        case "/debug":
                            isDebugEnabled = true;
                            goto IL_0216;
                        case "/silent":
                            verbosity = Verbosity.Silent;
                            goto IL_0216;
                        case "/verbose":
                            verbosity = Verbosity.Verbose;
                            goto IL_0216;
                        case "/ver":
                        case "/version":
                            PrintProductVersion();
                            flag = false;
                            break;

                        default:
                            if (_otherParsers.ContainsKey(argName))
                            {
                                _otherParsers[argName].WasInvokedByCommandLine = true;
                                flag = _otherParsers[argName].ParseCommandLineArgs(args, ref ii);
                            }
                            else
                            {
                                flag = false;
                            }

                            if (flag)
                            {
                                goto IL_0216;
                            }

                            Usage();
                            flag = false;
                            break;
                    }
                }
                else
                {
                    if (!flag)
                    {
                        break;
                    }

                    var num2 = 0;
                    foreach (var item in _otherParsers.Distinct(Comparer))
                    {
                        var argName = item.Key;
                        var value = item.Value;
                        if (value.WasInvokedByCommandLine)
                        {
                            num2++;
                            var flag2 = value.WereRequiredArgsProvided();
                            if (flag2.HasValue && !flag2.Value)
                            {
                                ConsoleOutput.PrintError("Error: \"{0}\": Missing required cmdline arguments!",
                                    argName);
                                flag = false;
                            }
                        }
                    }

                    if (!flag || num2 == 0)
                    {
                        flag = false;
                        Usage();
                    }
                }

                break;
            IL_0216:
                ii++;
            }
        }

        ConsoleOutput.Print();
        return flag;
    }

    public static void Usage()
    {
        PrintProductVersion();
        var text = "";
        var text2 = "";
        var text3 = "";
        var text4 = "";
        foreach (var item in _otherParsers.Values.Distinct())
        {
            text += item.GetHelpUsage();
            text2 += item.GetHelpTrigger();
            text4 += item.GetHelpExamples();
            var text5 = item.GetHelpDetails();
            if (!string.IsNullOrEmpty(text5))
            {
                text5 += "\r\n";
            }

            text3 += text5;
        }

        var newValue = new string(' ', ExecutingApplication.Name.Length);
        text = text.Replace("{Program.Name}", ExecutingApplication.Name).Replace("{_padding_len}", newValue);
        text2 = text2.Replace("{Program.Name}", ExecutingApplication.Name).Replace("{_padding_len}", newValue);
        text3 = text3.Replace("{Program.Name}", ExecutingApplication.Name).Replace("{_padding_len}", newValue);
        text4 = text4.Replace("{Program.Name}", ExecutingApplication.Name).Replace("{_padding_len}", newValue);
        text4 = text4.TrimEnd("\r\n".ToCharArray());
        ConsoleOutput.Print(Verbosity.Always.ToString(),
            "\r\nUsage:\r\n{0}\r\nGeneral cmdline parameters:\r\n  /silent          - Shows no cmdline output. (Check exit code for results.)\r\n  /verbose         - Shows GUID/Title (1 line) for updates offered by server.\r\n  /version         - Shows the program's version information, then exits.\r\n\r\n{4}{5} which mode(s) to use:\r\n{1}\r\n\r\n{2}\r\nExample command lines:\r\n\r\n{3}\r\n",
            text, text2, text3, text4, DerivedClassPreamble,
            string.IsNullOrEmpty(DerivedClassPreamble) ? "Specify" : "Next, specify");
    }

    internal static void Init()
    {
        _partialInitializers.ThreadSafeWrite(delegate
        {
            foreach (var partialInitializer in _partialInitializers)
            {
                if (!_partialInitializersExecuted[partialInitializer])
                {
                    partialInitializer();
                    _partialInitializersExecuted[partialInitializer] = true;
                }
            }
        });
    }

    protected static bool AddGuidsToList(string argValue, List<string> listGuids)
    {
        var flag = false;
        if (!string.IsNullOrEmpty(argValue))
        {
            var list = argValue.Split(",;".ToCharArray()).ToList();
            if (list.Count > 0)
            {
                listGuids.AddRange(list);
                flag = true;
            }
        }

        if (!flag)
        {
            Environment.ExitCode = 87;
        }

        return flag;
    }

    protected static string GetArgName(string[] args, ref int ii)
    {
        SplitArgNameValue(args, ref ii, out var argName, out var _);
        return argName.ToLowerInvariant();
    }

    protected static string GetArgValue(string[] args, ref int ii)
    {
        SplitArgNameValue(args, ref ii, out var _, out var argValue);
        if (string.IsNullOrEmpty(argValue) && ii + 1 < args.Length)
        {
            var text = args[ii + 1] ?? "";
            if (!text.StartsWith("/"))
            {
                ii++;
                argValue = text;
            }
        }

        return argValue ?? "";
    }

    protected static void PrintProductVersion()
    {
        var application = ThisAssembly.Application;
        ConsoleOutput.Print(Verbosity.Always.ToString(), "{0}\r\n{1}\r\n{2}Version {3}\r\n{4}\r\n", application.Name,
            application.Title, !string.IsNullOrEmpty(application.Description) ? application.Description + "\r\n" : "",
            application.InformationalVersion, application.Copyright);
    }

    protected static void ReportBadParameter(string label, string argName = null, string argValue = null,
        string suffix = null)
    {
        var text = "Error: ";
        var text2 = $"{text}{label}";
        if (!string.IsNullOrEmpty(argName))
        {
            text2 = $"{text}{argName}: {label}";
        }

        if (argValue != null)
        {
            text2 = text2 + "\"" + argValue + "\"";
        }

        if (suffix != null)
        {
            text2 = text2 + new string(' ', text.Length) + suffix + "\r\n";
        }

        Environment.ExitCode = 87;
        ConsoleOutput.PrintError(text2 + "!\r\n");
    }

    protected static bool SetEnumValue<TEnum>(ref TEnum variable, string newValue, string label) where TEnum : struct
    {
        var flag = Enum.TryParse(newValue, out variable);
        if (!flag)
        {
            ReportBadParameter(label, null, newValue,
                "       Legal values: [" +
                string.Join(", ", typeof(TEnum).GetEnumNames().Except(["Unknown"])) + "]");
            Environment.ExitCode = 1169;
        }

        return flag;
    }

    protected static void SplitArgNameValue(string[] args, ref int ii, out string argName, out string argValue)
    {
        argName = "";
        argValue = "";
        if (args == null || ii >= args.Length)
        {
            return;
        }

        var text = argName = args[ii];
        var num = text.IndexOf(":");
        if (num > 0)
        {
            argName = text.Substring(0, num);
            if (num < text.Length - 1)
            {
                argValue = text.Substring(num + 1);
            }
        }
    }

    internal class HandlerListComparer : IEqualityComparer<KeyValuePair<string, IParseCommandLineArgs>>
    {
        public bool Equals(KeyValuePair<string, IParseCommandLineArgs> x, KeyValuePair<string, IParseCommandLineArgs> y)
        {
            return Equals(x.Value, y.Value);
        }

        public int GetHashCode(KeyValuePair<string, IParseCommandLineArgs> obj)
        {
            if (obj.Value != null)
            {
                return obj.Value.GetHashCode();
            }

            return 0;
        }
    }
}