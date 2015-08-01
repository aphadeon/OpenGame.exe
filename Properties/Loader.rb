class Rect
  def to_a
    [self.x, self.y, self.width, self.height]
  end
end

class Plane
  def initialize(v = nil)
    @sprite = Sprite.new(v)
    @bitmap = nil
  end
 
  def dispose
	return if(@sprite.nil?)
	return if(@sprite.disposed?)
	@sprite.bitmap.dispose if(!@sprite.bitmap.nil? && !@sprite.bitmap.disposed?)
	@sprite.dispose
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

