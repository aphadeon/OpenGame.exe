def rgss_main
	begin
		yield
	rescue RGSSReset
		retry
	end
end
module Audio
  class <<self
    def setup_mdi
    end

    def bgm_play(filename, volume=100, pitch=100, pos=0)
    end

    def bgm_stop
    end

    def bgm_fade(time)
    end

    def bgm_pos

    end

    def bgs_play(filename, volume=100, pitch=100, pos=0)
    end

    def bgs_stop
    end

    def bgs_fade(time)
    end

    def bgs_pos

    end

    def me_play(filename, volume=100, pitch=100)
    end

    def me_stop
    end

    def me_fade(time)
    end

    def se_play(filename, volume=100, pitch=100)
    end

    def se_stop
    end
  end
end

class Rect
  def to_a
    [self.x, self.y, self.width, self.height]
  end
end

module Input
	def Input.get_code_from_symbol(sym)
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
		end
	end
	def Input.update
		RGSS::Input.update
		if(RGSS::Input.is_pressed(18))
			Graphics.reset
			raise RGSSReset
		end
	end
    def Input.press?(sym)
        return RGSS::Input.is_pressed(get_code_from_symbol(sym))
    end
    def Input.trigger?(sym)
		return RGSS::Input.is_triggered(get_code_from_symbol(sym))
    end
    def Input.repeat?(sym)
		return RGSS::Input.is_repeat(get_code_from_symbol(sym))
    end
    def Input.dir4
		return 4 if(press?(:LEFT))
		return 6 if(press?(:RIGHT))
		return 8 if(press?(:UP))
		return 2 if(press?(:DOWN))
		return 0
    end
    def Input.dir8
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

class Plane
  def initialize(v = nil)
    @sprite = Sprite.new(v)
    @bitmap = nil
  end
 
  def dispose
    b1 = @sprite.nil? || @sprite.disposed?
    b2 = b1 ? false : @sprite.bitmap.nil? || @sprite.bitmap.disposed?
    @sprite.bitmap.dispose if b2
    @sprite.dispose if b1
    return nil
  end
 
  def disposed?
    @sprite.nil? || @sprite.disposed?
  end
 
  def ox=(val)
    @sprite.ox = (val % (@bitmap.nil? ? 1 : @bitmap.width))
  end
 
  def oy=(val)
    @sprite.oy = (val % (@bitmap.nil? ? 1 : @bitmap.height))
  end
 
  def bitmap
    @bitmap
  end
 
  def bitmap=(bmp)
    w, h = vrect.width, vrect.height
   
    nw = bmp.width <= 100 ? 2 : 3
    nh = bmp.height <= 100 ? 2 : 3
   
    dx = [(w / bmp.width).ceil, 1].max * nw
    dy = [(h / bmp.height).ceil, 1].max * nh
 
    bw = dx * bmp.width
    bh = dy * bmp.height
 
    @bitmap = bmp
    @sprite.bitmap.dispose unless @sprite.bitmap.nil? or @sprite.bitmap.disposed?
    @sprite.bitmap = Bitmap.new(bw, bh)
   
    dx.times do |x|
      dy.times do |y|
        @sprite.bitmap.blt(x * bmp.width, y * bmp.height, @bitmap, @bitmap.rect)
      end
    end
  end
 
  def method_missing(sym, *argv, &argb)
    if @sprite.respond_to?(sym)
      return @sprite.send(sym, *argv, &argb)
    end
    super(sym, *argv, &argb)
  end
 
  private
  def vrect
    @sprite.viewport.nil? ? Rect.new(0, 0, Graphics.width, Graphics.height) :
    @sprite.viewport.rect
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
class RGSSReset < Exception
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
