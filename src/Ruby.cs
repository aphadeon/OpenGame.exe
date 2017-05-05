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

            try
            {
                engine.Execute(@"$RGSS_VERSION = " + Program.GetRuntime().GetRGSSVersion(), scope);
                engine.Execute(@"$GAME_DIRECTORY = '" + Program.GetRuntime().GetResourcePaths()[0].Replace(@"\", @"\\") + @"'", scope);
                engine.Execute(@"$GAME_OS_WIN = " + Program.GetRuntime().IsWindowsOS().ToString().ToLower(), scope);
            }
            catch (Exception e)
            {
                Program.Error(e.Message);
            }

            //Load system internals and our Ruby internals
            Console.WriteLine("Loading system");
            //engine.Execute(System.Text.Encoding.UTF8.GetString(Properties.Resources.System), scope);
            string script = System.Text.Encoding.UTF8.GetString(Properties.Resources.System);
            script = script.Substring(1);  //fix for a weird character that shouldn't be there o.O
            Eval(script);

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
            OpenGame.Runtime.Runtime.RGSSVersion = rtc.GetRGSSVersion();
            OpenGame.Runtime.Runtime.ResourcePaths = rtc.GetResourcePaths();
            OpenGame.Runtime.Runtime.DefaultResolutionWidth = rtc.GetDefaultResolutionWidth();
            OpenGame.Runtime.Runtime.DefaultResolutionHeight = rtc.GetDefaultResolutionHeight();
            OG_Graphics.initialize(Program.Window);
            if(rtc.IsDebug() && rtc.IsPlayTest()) engine.Execute("$DEBUG = $TEST = true", scope);
            engine.Execute(OG_Graphics.ruby_helper(), scope);
            engine.Execute(OG_Input.ruby_helper(), scope);
            engine.Execute(Plane.ruby_helper(), scope);
            engine.Execute(Table.ruby_helper(), scope);
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
                engine.Execute(@"$OPENGAME_EXE = true");
                engine.Execute("$RGSS_SCRIPTS_PATH = '" + rtc.GetScriptsPath() + "'");
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
