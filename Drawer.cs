using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

abstract class Drawer{
	protected Line[] lines;
	
	protected int width;
	protected int height;
	
	protected int sx;
	protected int sy;
	
	public Drawer(Line[] d){
		lines = d;
		
		figureSize();
	}
	
	protected abstract void figureSize();
	
	public abstract void draw(string file);
}

class ImageDrawer : Drawer{
	
	const int stepLen = 5;
	const int margin = 10;
	
	public ImageDrawer(Line[] d) : base(d){
		
	}
	
	protected override void figureSize(){
		int mx = 0, my = 0, ax = 0, ay = 0;
		
		int px = 0, py = 0;
		
		foreach(Line d in lines){
			switch(d.dir){
				case Dir.U:
					py--;
					my = Math.Min(my, py);
				break;
				
				case Dir.D:
					py++;
					ay = Math.Max(ay, py);
				break;
				
				case Dir.L:
					px--;
					mx = Math.Min(mx, px);
				break;
				
				case Dir.R:
					px++;
					ax = Math.Max(ax, px);
				break;
			}
		}
		
		width = 2*margin + stepLen * (ax - mx);
		height = 2*margin + stepLen * (ay - my);
		
		sx = margin - mx * stepLen;
		sy = margin - my * stepLen;
	}
	
	public override void draw(string file){
		try{
			using Bitmap bmp = new Bitmap(width, height);
			using Graphics g = Graphics.FromImage(bmp);
			
			g.Clear(Color.Black);
			
			int x = sx;
			int y = sy;
			
			using Pen pen = new Pen(Color.White, 1);
			
			foreach(Line d in lines){
				int newX = x, newY = y;
				switch(d.dir){
					case Dir.U:
						newY -= stepLen;
					break;
					
					case Dir.D:
						newY += stepLen;
					break;
					
					case Dir.L:
						newX -= stepLen;
					break;
					
					case Dir.R:
						newX += stepLen;
					break;
				}
				
				g.DrawLine(pen, x, y, newX, newY);
				x = newX;
				y = newY;
			}
			
			// save to file
			bmp.Save(file, ImageFormat.Png);
		}catch(Exception e){
			Console.Error.WriteLine("Error drawing: " + e);
		}
	}
}

class TextDrawer : Drawer{
	
	const int margin = 1;
	
	public TextDrawer(Line[] d) : base(d){
		
	}
	
	protected override void figureSize(){
		int mx = 0, my = 0, ax = 0, ay = 0;
		
		int px = 0, py = 0;
		
		foreach(Line d in lines){
			switch(d.dir){
				case Dir.U:
					py--;
					my = Math.Min(my, py);
				break;
				
				case Dir.D:
					py++;
					ay = Math.Max(ay, py);
				break;
				
				case Dir.L:
					px--;
					mx = Math.Min(mx, px);
				break;
				
				case Dir.R:
					px++;
					ax = Math.Max(ax, px);
				break;
			}
		}
		
		width = 2*margin + (ax - mx);
		height = 2*margin + (ay - my);
		
		sx = margin - mx;
		sy = margin - my;
	}
	
	public override void draw(string file){
		char[,] buffer = new char[width, height];
		Conn[,] conn = new Conn[width, height];
		
		int x = sx;
		int y = sy;
		
		foreach (var line in lines) {
			int dx = 0, dy = 0;
			switch (line.dir) {
				case Dir.U: dy = -1; break;
				case Dir.D: dy =  1; break;
				case Dir.L: dx = -1; break;
				case Dir.R: dx =  1; break;
			}
			
			int nx = x + dx;
			int ny = y + dy;
			
			// mark both ends
			if (dx == -1) { conn[x,y] |= Conn.Left; conn[nx,ny] |= Conn.Right; }
			if (dx ==  1) { conn[x,y] |= Conn.Right; conn[nx,ny] |= Conn.Left; }
			if (dy == -1) { conn[x,y] |= Conn.Up;    conn[nx,ny] |= Conn.Down; }
			if (dy ==  1) { conn[x,y] |= Conn.Down;  conn[nx,ny] |= Conn.Up; }
			
			// advance
			x = nx;
			y = ny;
		}
		
		
		for (int j = 0; j < height; j++) {
			for (int i = 0; i < width; i++) {
				buffer[i,j] = ConnToChar(conn[i,j]);
			}
		}
		
		StringBuilder sb = new(width * height + height);
		
		for(int j = 0; j < height; j++){
			for(int i = 0; i < width; i++){
				sb.Append(buffer[i, j]);
			}
			sb.AppendLine();
		}
		
		// save to file
		File.WriteAllText(file, sb.ToString());
	}
	
	static char ConnToChar(Conn c){
		return c switch {
			Conn.Up | Conn.Down             => '│',
			Conn.Left | Conn.Right          => '─',
			Conn.Down | Conn.Right          => '┌',
			Conn.Down | Conn.Left           => '┐',
			Conn.Up   | Conn.Right          => '└',
			Conn.Up   | Conn.Left           => '┘',
			Conn.Up | Conn.Left | Conn.Right => '┴',
			Conn.Down | Conn.Left | Conn.Right => '┬',
			Conn.Up | Conn.Down | Conn.Left => '┤',
			Conn.Up | Conn.Down | Conn.Right => '├',
			Conn.Up | Conn.Down | Conn.Left | Conn.Right => '┼',
			_ => ' '
		};
	}
}

[Flags]
enum Conn {
    None  = 0,
    Up    = 1,
    Down  = 2,
    Left  = 4,
    Right = 8
}
