# Load required builtins for RGSS
load_assembly 'IronRuby.Libraries', 'IronRuby.StandardLibrary.Zlib'
if $GAME_OS_WIN
	load_assembly 'IronRuby.Libraries', 'IronRuby.StandardLibrary.Win32API'
end
load_assembly 'IronRuby.Libraries', 'IronRuby.StandardLibrary.Threading'
require 'System\OpenGame.Runtime.dll'

# Set the cwd to the game data path
Dir.chdir($GAME_DIRECTORY)

# API compatible Thread based Fiber implementation for Ruby 1.8
# https://gist.github.com/tmm1/4631
# (c) 2008 Aman Gupta (tmm1), used with permission

unless defined? Fiber
 
  class FiberError < StandardError
  end
 
  class Fiber
    def initialize
      raise ArgumentError, 'new Fiber requires a block' unless block_given?
 
      @yield = Queue.new
      @resume = Queue.new
 
      @thread = Thread.new{ @yield.push [ *yield(*@resume.pop) ] }
      @thread.abort_on_exception = true
      @thread[:fiber] = self
    end
    attr_reader :thread
 
    def resume *args
      raise FiberError, 'dead fiber called' unless @thread.alive?
      @resume.push(args)
      result = @yield.pop
      result.size > 1 ? result : result.first
    end
    
    def yield *args
      @yield.push(args)
      result = @resume.pop
      result.size > 1 ? result : result.first
    end
    
    def self.yield *args
      raise FiberError, "can't yield from root fiber" unless fiber = Thread.current[:fiber]
      fiber.yield(*args)
    end
 
    def self.current
      Thread.current[:fiber] or raise FiberError, 'not inside a fiber'
    end
 
    def inspect
      "#<#{self.class}:0x#{self.object_id.to_s(16)}>"
    end
  end
end

# Default RGSS globals

class RGSSReset < Exception
end

def rgss_main
	begin
		yield
	rescue RGSSReset
		retry
	end
end

module Kernel
	def load_data(filename)
		File.open(filename, "rb") { |f|
		  obj = Marshal.load(f)
		}
	end

	def save_data(filename)
		File.open(filename, "wb") { |f|
		  Marshal.dump(obj, f)
		}
	end
end

class Table
  attr_reader :xsize, :ysize, :zsize

  def initialize(xsize, ysize=1, zsize=1)
    @xsize = xsize
    @ysize = ysize
    @zsize = zsize
    @data  = Array.new(@xsize*@ysize*@zsize, 0)
  end

  def resize(xsize, ysize=1, zsize=1)
  end

  def [](x, y=0, z=0)
    return nil if x >= @xsize or y >= @ysize
    @data[x + y * @xsize + z * @xsize * @ysize]
  end

  def []= (x, y, z, v)
    @data[x + y * @xsize + z * @xsize * @ysize]=v
  end

  def self._load(s)
    Table.new(1).instance_eval {
      @size, @xsize, @ysize, @zsize, xx, *@data = s.unpack('LLLLLS*')
      self
    }
  end

  def _dump(d = 0)
    [@size, @xsize, @ysize, @zsize, @xsize*@ysize*@zsize, *@data].pack('LLLLLS*')
  end

end


# Script loader

def rgss_start
	filePath = 'Data/Scripts.rxdata' if $RGSS_VERSION == 1
	filePath = 'Data/Scripts.rvdata' if $RGSS_VERSION == 2
	filePath = 'Data/Scripts.rvdata2' if $RGSS_VERSION == 3
    $RGSS_SCRIPTS = []
    unknownScript = 0
   
    # Open the script file
	begin
		script = File.new(filePath, 'r')
	rescue StandardError => error
		p "IO failed: " + $!
	end

    # Gets the array of scripts
    rawData = Marshal.load(script)

    # inflate each script
    rawData.each do |dataArray|
        data = ""
        begin
            data = Zlib::Inflate.inflate(dataArray[2])
        rescue StandardError => error
            data = ""
        end
        
        unless(data.strip.length == 0)
            # Fix up the name
            if(dataArray[1].strip.length == 0)
                dataArray[1] = "Unknown #{unknownScript}"
                unknownScript += 1
            end
        end
		if(data.strip.length > 0) 
			puts "Loaded script: " + dataArray[1]
			$RGSS_SCRIPTS.push([dataArray[0], dataArray[1], dataArray[2], data])
		end
    end
    
	out = ""
	begin
		$RGSS_SCRIPTS.each { |script|
			$0 = script[1];
			rgss_exec script[3], script[1]
		}
	rescue SyntaxError, NameError => boom
		str = "Script compiler error:\n " + boom
		out += str + "\n"
		out += boom.backtrace.join("\n") + "\n"
	rescue StandardError => bang
		str = "Script runtime error:\n " + bang
		out += str + "\n"
		out += bang.backtrace.join("\n") + "\n"
	rescue
		out += "Unknown error happened"
	end
	if(out != "")
		out = out.split(":in `rgss_exec'")[0]
		raise out
	end
end

def rgss_exec(script, scriptname)
	eval(script, nil, scriptname, 0)
end
