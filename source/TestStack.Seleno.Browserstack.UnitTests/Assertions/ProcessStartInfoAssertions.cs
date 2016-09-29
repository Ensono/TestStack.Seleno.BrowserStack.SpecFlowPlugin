using System.Diagnostics;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace TestStack.Seleno.Browserstack.UnitTests.Assertions
{
    public class ProcessStartInfoAssertions : ReferenceTypeAssertions<ProcessStartInfo, ProcessStartInfoAssertions>
    {
        protected override string Context { get { return "ProcessStartInfo"; } }

        public ProcessStartInfoAssertions(ProcessStartInfo processStartInfo)
        {
            Subject = processStartInfo;
        }

        public void MatchWith(string binaryAbsolutePath, string arguments, bool createNoWindow = true,
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Hidden, bool redirectStandardOutput = true,
            bool redirectStandardError = true, bool redirectStandardInput = true, bool useShellExecute = false)
        {
            Execute
                .Assertion
                .ForCondition(Subject.FileName == binaryAbsolutePath &&
                              Subject.Arguments == arguments &&
                              Subject.CreateNoWindow == createNoWindow &&
                              Subject.WindowStyle == windowStyle &&
                              Subject.RedirectStandardOutput == redirectStandardOutput &&
                              Subject.RedirectStandardError == redirectStandardError &&
                              Subject.RedirectStandardInput == redirectStandardInput &&
                              Subject.UseShellExecute == useShellExecute)
                .FailWith($@"Expected StartInfo process have 
{Expected(binaryAbsolutePath,arguments, createNoWindow, windowStyle, redirectStandardOutput, redirectStandardError, redirectStandardInput, useShellExecute)}, 
but found:
{FormatProcessStartInfo(Subject)}.");
        }

        private string FormatProcessStartInfo(ProcessStartInfo subject)
        {
            return $@"{{ 
                         FileName=""{subject.FileName}"",
                         Arguments=""{subject.Arguments}"",
                         CreateNoWindow={subject.CreateNoWindow},
                         WindowStyle={subject.WindowStyle},
                         RedirectStandardOutput={subject.RedirectStandardOutput},
                         RedirectStandardError={subject.RedirectStandardError},
                         RedirectStandardInput={subject.RedirectStandardInput},
                         UseShellExecute={subject.UseShellExecute},                
                      }}";
        }

        private string Expected(string binaryAbsolutePath, string arguments, bool createNoWindow = true,
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Hidden, bool redirectStandardOutput = true,
            bool redirectStandardError = true, bool redirectStandardInput = true, bool useShellExecute = false)
        {
            return $@"{{ 
                         FileName=""{binaryAbsolutePath}"",
                         Arguments=""{arguments}"",
                         CreateNoWindow={createNoWindow},
                         WindowStyle={windowStyle},
                         RedirectStandardOutput={redirectStandardOutput},
                         RedirectStandardError={redirectStandardError},
                         RedirectStandardInput={redirectStandardInput},
                         UseShellExecute={useShellExecute},                
                      }}";
        }
    }
}