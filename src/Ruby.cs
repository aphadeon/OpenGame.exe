using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGame
{
    class Ruby
    {
        private ScriptRuntime runtime;
        private ScriptEngine engine;
        private ScriptScope scope;

        public Ruby()
        {
            //Setup the script engine runtime
            var setup = new ScriptRuntimeSetup();
            setup.LanguageSetups.Add(
                new LanguageSetup(
                    "IronRuby.Runtime.RubyContext, IronRuby",
                    "IronRuby 1.0",
                    new[] { "IronRuby", "Ruby", "rb" },
                    new[] { ".rb" }));
            setup.DebugMode = true;
            
            //Create the runtime, engine, and scope
            runtime = ScriptRuntime.CreateRemote(AppDomain.CurrentDomain, setup);
            engine = runtime.GetEngine("Ruby");
            scope = engine.CreateScope();

            //Load system internals and our Ruby internals
            Eval(System.Text.Encoding.Default.GetString(Properties.Resources.System));
            Eval(System.Text.Encoding.Default.GetString(Properties.Resources.Loader));

            //Load the version appropriate RPG datatypes
            //TODO: Add other RPG datatype versions
            if (Program.GetRGSSVersion() == 3)
            {
                Eval(System.Text.Encoding.Default.GetString(Properties.Resources.RPG3));
            }
        }

        public void Start()
        {
            Graphics.initialize();
            engine.Execute(Window.ruby_helper(), scope);
            engine.Execute(CTilemap.ruby_helper(), scope);
            engine.Execute(Viewport.ruby_helper(), scope);
            engine.Execute(Rect.ruby_helper(), scope);
            engine.Execute(Sprite.ruby_helper(), scope);
            engine.Execute(Color.ruby_helper(), scope);
            engine.Execute(Tone.ruby_helper(), scope);
            engine.Execute(Bitmap.ruby_helper(), scope);
            Font.load_fonts();
            engine.Execute(Font.ruby_helper(), scope);
            Eval(@"rgss_start");
        }

        public void Eval(string str)
        {
            try
            {
                var source = engine.CreateScriptSourceFromString(str);
                source.Compile(new ReportingErrorListener());
                source.Execute(scope);
            }
            catch (Exception e)
            {
                Program.Error(e.Message);
            }
        }

        public void Dispose()
        {
            scope = null;
            engine.Runtime.Shutdown();
            engine = null;
        }
    }

    public class ReportingErrorListener : ErrorListener
    {
        public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            Program.Error(message);
        }
    }
}
