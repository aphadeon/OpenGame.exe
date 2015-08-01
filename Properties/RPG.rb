# RPG datatypes

module RPG
end

#RGSS 1 Exclusives
if $RGSS_VERSION == 1
	module RPG::Cache
		@cache = {}
		def self.load_bitmap(folder_name, filename, hue = 0)
			path = folder_name + filename
			if not @cache.include?(path) or @cache[path].disposed?
			if filename != ''
				@cache[path] = Bitmap.new(path)
			else
				@cache[path] = Bitmap.new(32, 32)
			end
			end
			if hue == 0
			@cache[path]
			else
			key = [path, hue]
			if not @cache.include?(key) or @cache[key].disposed?
				@cache[key] = @cache[path].clone
				@cache[key].hue_change(hue)
			end
			@cache[key]
			end
		end
		def self.animation(filename, hue)
			self.load_bitmap('Graphics/Animations/', filename, hue)
		end
		def self.autotile(filename)
			self.load_bitmap('Graphics/Autotiles/', filename)
		end
		def self.battleback(filename)
			self.load_bitmap('Graphics/Battlebacks/', filename)
		end
		def self.battler(filename, hue)
			self.load_bitmap('Graphics/Battlers/', filename, hue)
		end
		def self.character(filename, hue)
			self.load_bitmap('Graphics/Characters/', filename, hue)
		end
		def self.fog(filename, hue)
			self.load_bitmap('Graphics/Fogs/', filename, hue)
		end
		def self.gameover(filename)
			self.load_bitmap('Graphics/Gameovers/', filename)
		end
		def self.icon(filename)
			self.load_bitmap('Graphics/Icons/', filename)
		end
		def self.panorama(filename, hue)
			self.load_bitmap('Graphics/Panoramas/', filename, hue)
		end
		def self.picture(filename)
			self.load_bitmap("Graphics/Pictures/", filename)
		end
		def self.tileset(filename)
			self.load_bitmap('Graphics/Tilesets/', filename)
		end
		def self.title(filename)
			self.load_bitmap('Graphics/Titles/', filename)
		end
		def self.windowskin(filename)
			self.load_bitmap('Graphics/Windowskins/', filename)
		end
		def self.tile(filename, tile_id, hue)
			key = [filename, tile_id, hue]
			if not @cache.include?(key) or @cache[key].disposed?
			@cache[key] = Bitmap.new(32, 32)
			x = (tile_id - 384) % 8 * 32
			y = (tile_id - 384) / 8 * 32
			rect = Rect.new(x, y, 32, 32)
			@cache[key].blt(0, 0, self.tileset(filename), rect)
			@cache[key].hue_change(hue)
			end
			@cache[key]
		end
		def self.clear
			@cache = {}
			GC.start
		end
	end
end

class RPG::Map
	def initialize(width, height)
		@width = width
		@height = height
		@bgm = $RGSS_VERSION < 3 ? RPG::AudioFile.new : RPG::BGM.new
		@bgs = $RGSS_VERSION < 3 ? RPG::AudioFile.new('', 80) : RPG::BGS.new('', 80)
		@autoplay_bgm = false
		@autoplay_bgs = false
		@events = {}
		@encounter_list = []
		@encounter_step = 30
		@data = Table.new(width, height, $RGSS_VERSION == 3 ? 4 : 3)
		@tileset_id = 1 if $RGSS_VERSION != 2
		if $RGSS_VERSION > 1
			@disable_dashing = false
			@scroll_type = 0
			@parallax_name = ''
			@parallax_loop_x = false
			@parallax_loop_y = false
			@parallax_sx = 0
			@parallax_sy = 0
			@parallax_show = false
			if $RGSS_VERSION == 3
				@display_name = ''
				@specify_battleback = false
				@battleback_floor_name = ''
				@battleback_wall_name = ''
				@note = ''
			end
		end
	end
    attr_accessor :width
    attr_accessor :height
	attr_accessor :bgm
	attr_accessor :bgs
	attr_accessor :autoplay_bgm
    attr_accessor :autoplay_bgs
	attr_accessor :events
    attr_accessor :data
    attr_accessor :encounter_list
    attr_accessor :encounter_step
	if $RGSS_VERSION != 2
	  attr_accessor :tileset_id
	end
	if $RGSS_VERSION > 1
		attr_accessor :disable_dashing
		attr_accessor :scroll_type
		attr_accessor :parallax_name
		attr_accessor :parallax_loop_x
		attr_accessor :parallax_loop_y
		attr_accessor :parallax_sx
		attr_accessor :parallax_sy
		attr_accessor :parallax_show
		if $RGSS_VERSION == 3
			attr_accessor :display_name
			attr_accessor :specify_battleback
			attr_accessor :battleback1_name
			attr_accessor :battleback2_name
			attr_accessor :note
		end
	end
end

class RPG::MapInfo
	def initialize
		@name = ''
		@parent_id = 0
		@order = 0
		@expanded = false
		@scroll_x = 0
		@scroll_y = 0
	end
    attr_accessor :name
    attr_accessor :parent_id
    attr_accessor :order
    attr_accessor :expanded
    attr_accessor :scroll_x
    attr_accessor :scroll_y
end

class RPG::Event
	def initialize(x, y)
		@id = 0
		@name = ''
		@x = x
		@y = y
		@pages = [RPG::Event::Page.new]
	end
	attr_accessor :id
	attr_accessor :name
	attr_accessor :x
	attr_accessor :y
	attr_accessor :pages
end

class RPG::Event::Page
	def initialize
		@condition = RPG::Event::Page::Condition.new
		@graphic = RPG::Event::Page::Graphic.new
		@move_type = 0
		@move_speed = 3
		@move_frequency = 3
		@move_route = RPG::MoveRoute.new
		@walk_anime = true
		@step_anime = false
		@direction_fix = false
		@through = false
		@trigger = 0
		@list = [RPG::EventCommand.new]
		if $RGSS_VERSION == 1
			@always_on_top = false
		else
			@priority_type = 0
		end
	end
	attr_accessor :condition
	attr_accessor :graphic
	attr_accessor :move_type
	attr_accessor :move_speed
	attr_accessor :move_frequency
	attr_accessor :move_route
	attr_accessor :walk_anime
	attr_accessor :step_anime
	attr_accessor :direction_fix
	attr_accessor :through
	attr_accessor :trigger
	attr_accessor :list
	if $RGSS_VERSION == 1
		attr_accessor :always_on_top
	else
		attr_accessor :priority_type
	end
end