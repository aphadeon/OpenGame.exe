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
            Console.WriteLine("Loading system");
            //engine.Execute(System.Text.Encoding.UTF8.GetString(Properties.Resources.System), scope);
            string script = System.Text.Encoding.UTF8.GetString(Properties.Resources.System);
            script = script.Substring(1);  //fix for a weird character that shouldn't be there o.O
            Eval(script);
            script = System.Text.Encoding.UTF8.GetString(Properties.Resources.Loader);
            script = script.Substring(1); //fix for weird initial character
            Eval(script);

            try
            {
                engine.Execute(@"$RGSS_VERSION = " + Program.GetRuntime().GetRGSSVersion(), scope);
            }
            catch (Exception e)
            {
                Program.Error(e.Message);
            }

            //Load the adaptable RPG datatypes
            script = System.Text.Encoding.UTF8.GetString(Properties.Resources.RPG);
            script = script.Substring(1);
            Eval(script);

            //Load the version appropriate RPG datatypes
            if (Program.GetRuntime().GetRGSSVersion() == 1)
            {
                script = System.Text.Encoding.UTF8.GetString(Properties.Resources.RPG1);
                script = script.Substring(1);
                Eval(script);
            }
            if (Program.GetRuntime().GetRGSSVersion() == 2)
            {
                script = System.Text.Encoding.UTF8.GetString(Properties.Resources.RPG2);
                script = script.Substring(1);
                Eval(script);
            }
            if (Program.GetRuntime().GetRGSSVersion() == 3)
            {
                script = System.Text.Encoding.UTF8.GetString(Properties.Resources.RPG3);
                script = script.Substring(1);
                Eval(script);
            }
        }

        public void Start()
        {
            RuntimeConfiguration rtc = Program.GetRuntime();
            Runtime.RGSSVersion = rtc.GetRGSSVersion();
            Runtime.ResourcePaths = rtc.GetResourcePaths();
            Runtime.DefaultResolutionWidth = rtc.GetDefaultResolutionWidth();
            Runtime.DefaultResolutionHeight = rtc.GetDefaultResolutionHeight();
            Graphics.initialize(Program.Window);
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


            try
            {
                engine.Execute(@"rgss_start", scope);
            }
            catch (Exception e)
            {
                Program.Error(e.Message);
            }
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
