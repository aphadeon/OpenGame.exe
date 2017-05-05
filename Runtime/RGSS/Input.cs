using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OG_Input
{

    private static int[] KeyCounters = new int[22];
    private static int RepeatTimeInFrames = 10;

    public static void update_internal()
    {
        if (!OG_Graphics.Window.IsExiting) OG_Graphics.Window.ProcessEvents();

        OpenTK.Input.KeyboardDevice kb = OG_Graphics.Window.Keyboard;
        KeyCounters[0] = (kb[OpenTK.Input.Key.S] || kb[OpenTK.Input.Key.Down]) ? KeyCounters[0] + 1 : 0;
        KeyCounters[1] = (kb[OpenTK.Input.Key.A] || kb[OpenTK.Input.Key.Left]) ? KeyCounters[1] + 1 : 0;
        KeyCounters[2] = (kb[OpenTK.Input.Key.D] || kb[OpenTK.Input.Key.Right]) ? KeyCounters[2] + 1 : 0;
        KeyCounters[3] = (kb[OpenTK.Input.Key.W] || kb[OpenTK.Input.Key.Up]) ? KeyCounters[3] + 1 : 0;
        KeyCounters[4] = (kb[OpenTK.Input.Key.LShift]) ? KeyCounters[4] + 1 : 0;
        KeyCounters[5] = (kb[OpenTK.Input.Key.X] || kb[OpenTK.Input.Key.Escape]) ? KeyCounters[5] + 1 : 0;
        KeyCounters[6] = (kb[OpenTK.Input.Key.Z] || kb[OpenTK.Input.Key.Enter]) ? KeyCounters[6] + 1 : 0;
        KeyCounters[7] = (kb[OpenTK.Input.Key.PageUp]) ? KeyCounters[7] + 1 : 0;
        KeyCounters[8] = (kb[OpenTK.Input.Key.PageDown]) ? KeyCounters[8] + 1 : 0;
        KeyCounters[9] = (kb[OpenTK.Input.Key.LShift] || kb[OpenTK.Input.Key.RShift]) ? KeyCounters[9] + 1 : 0;
        KeyCounters[10] = (kb[OpenTK.Input.Key.LControl] || kb[OpenTK.Input.Key.RControl]) ? KeyCounters[10] + 1 : 0;
        KeyCounters[11] = (kb[OpenTK.Input.Key.LAlt] || kb[OpenTK.Input.Key.RAlt]) ? KeyCounters[11] + 1 : 0;
        KeyCounters[12] = (kb[OpenTK.Input.Key.F5]) ? KeyCounters[12] + 1 : 0;
        KeyCounters[13] = (kb[OpenTK.Input.Key.F6]) ? KeyCounters[13] + 1 : 0;
        KeyCounters[14] = (kb[OpenTK.Input.Key.F7]) ? KeyCounters[14] + 1 : 0;
        KeyCounters[15] = (kb[OpenTK.Input.Key.F8]) ? KeyCounters[15] + 1 : 0;
        KeyCounters[16] = (kb[OpenTK.Input.Key.F9]) ? KeyCounters[16] + 1 : 0;
        KeyCounters[17] = (kb[OpenTK.Input.Key.F2]) ? KeyCounters[17] + 1 : 0;
        KeyCounters[18] = (kb[OpenTK.Input.Key.F12]) ? KeyCounters[18] + 1 : 0;
        KeyCounters[19] = (kb[OpenTK.Input.Key.B]) ? KeyCounters[19] + 1 : 0;
        KeyCounters[20] = (kb[OpenTK.Input.Key.S]) ? KeyCounters[20] + 1 : 0;
        KeyCounters[21] = (kb[OpenTK.Input.Key.D]) ? KeyCounters[21] + 1 : 0;
    }

    public static bool is_pressed(int code)
    {
        return KeyCounters[code] > 0;
    }

    public static bool is_triggered(int code)
    {
        return KeyCounters[code] == 1;
    }

    public static bool is_repeat(int code)
    {
        return KeyCounters[code] == 2 || KeyCounters[code]> RepeatTimeInFrames;
    }

    public static string ruby_helper()
    {
        //IronRuby.Builtins.RubySymbol s = new IronRuby.Builtins.RubySymbol();
        return @"
            module Input
                class << self
	                Input::DOWN = 0
	                Input::LEFT = 1
	                Input::RIGHT = 2
	                Input::UP = 3
	                Input::A = 4
	                Input::B = 5
	                Input::C = 6
	                Input::L = 7
	                Input::R = 8
	                Input::SHIFT = 9
	                Input::CTRL = 10
	                Input::ALT = 11
	                Input::F5 = 12
	                Input::F6 = 13
	                Input::F7 = 14
	                Input::F8 = 15
	                Input::F9 = 16
                    Input::X = 19
                    Input::Y = 20
                    Input::Z = 21

                    def update
                        OG_Input.update
                    end
                    def press?(sym)
                        OG_Input.press?(sym)
                    end
                    def trigger?(sym)
                        OG_Input.trigger?(sym)
                    end
                    def repeat?(sym)
                        OG_Input.repeat?(sym)
                    end
                    def dir4
                        OG_Input.dir4
                    end
                    def dir8
                        OG_Input.dir8
                    end
                end
            end
            class OG_Input
	            OG_Input::DOWN = 0
	            OG_Input::LEFT = 1
	            OG_Input::RIGHT = 2
	            OG_Input::UP = 3
	            OG_Input::A = 4
	            OG_Input::B = 5
	            OG_Input::C = 6
	            OG_Input::L = 7
	            OG_Input::R = 8
	            OG_Input::SHIFT = 9
	            OG_Input::CTRL = 10
	            OG_Input::ALT = 11
	            OG_Input::F5 = 12
	            OG_Input::F6 = 13
	            OG_Input::F7 = 14
	            OG_Input::F8 = 15
	            OG_Input::F9 = 16
                OG_Input::X = 19
                OG_Input::Y = 20
                OG_Input::Z = 21

	            def OG_Input.get_code_from_symbol(sym)
		            case sym
		            when :DOWN then return 0
		            when :LEFT then return 1
		            when :RIGHT then return 2
		            when :UP then return 3
		            when :A then return 4
		            when :B then return 5
		            when :C then return 6
		            when :L then return 7
		            when :R then return 8
		            when :SHIFT then return 9
		            when :CTRL then return 10
		            when :ALT then return 11
		            when :F5 then return 12
		            when :F6 then return 13
		            when :F7 then return 14
		            when :F8 then return 15
		            when :F9 then return 16
                    when :X then return 19
                    when :Y then return 20
                    when :Z then return 21
		            end
	            end
	            def OG_Input.update
		            update_internal()
		            if(is_pressed(18))
			            OG_Graphics.reset
			            raise RGSSReset
		            end
	            end
                def OG_Input.press?(sym)
	              case sym
		            when Symbol
		              return is_pressed(get_code_from_symbol(sym))
		            else
		              return is_pressed(sym)
	               end
                end
                def OG_Input.trigger?(sym)
	                case sym
		            when Symbol
		              return is_triggered(get_code_from_symbol(sym))
		            else
		              return is_triggered(sym)
	               end
                end
                def OG_Input.repeat?(sym)
		            case sym
		            when Symbol
		              return is_repeat(get_code_from_symbol(sym))
		            else
		              return is_repeat(sym)
	               end
                end
                def OG_Input.dir4
		            return 4 if(press?(:LEFT))
		            return 6 if(press?(:RIGHT))
		            return 8 if(press?(:UP))
		            return 2 if(press?(:DOWN))
		            return 0
                end
                def OG_Input.dir8
		            return 1 if(press?(:LEFT) && press?(:DOWN))
		            return 3 if(press?(:RIGHT) && press?(:DOWN))
		            return 7 if(press?(:LEFT) && press?(:UP))
		            return 9 if(press?(:RIGHT) && press?(:UP))
		            return 4 if(press?(:LEFT))
		            return 6 if(press?(:RIGHT))
		            return 8 if(press?(:UP))
		            return 2 if(press?(:DOWN))
		            return 0
                end
            end
        ";
    }
}
