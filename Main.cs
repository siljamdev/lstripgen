using System;
using System.Text;
using System.Diagnostics;

class Program{
	public static void Main(string[] args){
		if(args.Length == 0){
			Console.WriteLine("Use -h for help");
			return;
		}
		
		int n = 0;
		
		string rules = null;
		int? iter = null;
		
		bool fileSp = false;
		string outFile = "%.png";
		
		bool text = false;
		bool notOpen = true;
		
		bool sSet = false;
		string s = "0U";
		
		bool drawOnlyLast = false;
		
		while(n < args.Length){
			if(args[n] == "-r"){
				n++;
				
				if(rules != null){
					Console.Error.WriteLine("Rules already specified");
					return;
				}
				
				if(n >= args.Length){
					Console.Error.WriteLine("Expected following string after '-r'");
					return;
				}
				
				rules = args[n];
			}else if(args[n] == "-i"){
				n++;
				
				if(iter != null){
					Console.Error.WriteLine("Number of iterations already specified");
					return;
				}
				
				if(n >= args.Length){
					Console.Error.WriteLine("Expected following number after '-i'");
					return;
				}
				
				if(!short.TryParse(args[n], out short nnn)){
					Console.Error.WriteLine("Invalid number: " + args[n]);
					return;
				}
				
				iter = nnn;
			}else if(args[n] == "-o"){
				n++;
				
				if(fileSp){
					Console.Error.WriteLine("Output file already specified");
					return;
				}
				
				fileSp = true;
				
				if(n >= args.Length){
					Console.Error.WriteLine("Expected following path after '-o'");
					return;
				}
				
				outFile = args[n];
			}else if(args[n] == "-t"){
				if(text){
					Console.Error.WriteLine("Text mode already specified");
					return;
				}
				
				text = true;
				
				if(!fileSp){
					outFile = "%.txt";
				}
			}else if(args[n] == "-l"){
				if(drawOnlyLast){
					Console.Error.WriteLine("Draw only last mode already specified");
					return;
				}
				
				drawOnlyLast = true;
			}else if(args[n] == "-s"){
				n++;
				
				if(sSet){
					Console.Error.WriteLine("Start strip already specified");
					return;
				}
				
				sSet = true;
				
				if(n >= args.Length){
					Console.Error.WriteLine("Expected following string after '-s'");
					return;
				}
				
				s = args[n];
			}else if(args[n] == "-p"){
				if(!notOpen){
					Console.Error.WriteLine("Open mode already specified");
					return;
				}
				
				notOpen = false;
			}else if(args[n] == "-h"){
				printHelp();
			}else{
				Console.Error.WriteLine("Unknown option: '" + args[n] + "'. Use -h for help");
				return;
			}
			n++;
		}
		
		if(rules == null){
			Console.Error.WriteLine("No rules provided. Use '-r'");
			return;
		}
		
		if(iter == null){
			Console.Error.WriteLine("No iteration count provided. Use '-i'");
			return;
		}
		
		if(iter <= 0){
			Console.Error.WriteLine("Iteration count must be greater than 0.");
			return;
		}
		
		int c = 0;
		int n2 = (int) iter;
		
		IIterator iterator;
		
		try{
			iterator = new CustomIterator(rules);
		}catch(Exception e){
			Console.Error.WriteLine("Rules error: " + e.Message);
			return;
		}
		
		Line[] t;
		
		try{
			t = parseSeq(s);
		}catch(Exception e){
			Console.Error.WriteLine("Starting strip error: " + e.Message);
			return;
		}
		
		if(!drawOnlyLast){
			for(int i = 0; i < n2; i++){
				if(i != 0){
					t = iterator.iterate(t);
				}
				
				Drawer d;
				if(text){
					d = new TextDrawer(t);
				}else{
					d = new ImageDrawer(t);
				}
				
				string fffff = outFile.Replace("%", i.ToString());
				
				d.draw(fffff);
				
				if(!notOpen){
					ProcessStartInfo psi = new ProcessStartInfo{
						FileName = fffff,
						UseShellExecute = true
					};
					Process.Start(psi);
				}
			}
		}else{
			int i;
			for(i = 0; i < n2; i++){
				if(i != 0){
					t = iterator.iterate(t);
				}
			}
			
			i--;
			
			Drawer d;
			if(text){
				d = new TextDrawer(t);
			}else{
				d = new ImageDrawer(t);
			}
			
			string fffff = outFile.Replace("%", i.ToString());
			
			d.draw(fffff);
			
			if(!notOpen){
				ProcessStartInfo psi = new ProcessStartInfo{
					FileName = fffff,
					UseShellExecute = true
				};
				Process.Start(psi);
			}
		}
	}
	
	static void printHelp(){
		Console.WriteLine("### LineStrip Generator by siljamdev ###");
		Console.WriteLine();
		Console.WriteLine("HELP");
		Console.WriteLine("LineStrip Generator generates linestrips in a series of iterations. Each line has a direction and a flavor. Flavor is a number used internally to produce more complex patterns.");
		Console.WriteLine("Each iteration replaces each line (original) with 1 or more lines (replacement)");
		Console.WriteLine("Example rules (Hilbert curve): 1X:1X;2X:;0U/0D:0C2X2W1X0X2C2E1C0X2C2E1E0W2E2C;0L/0R:0W2X2C1X0X2W2E1W0X2W2E1E0C2E2W;");
		Console.WriteLine();
		Console.WriteLine("Usage:");
		Console.WriteLine("lstripgen -r <rules> -i <iter num> [-s <starting strip>] [-o <output path>] [-t] [-p] [-l] [-h]");
		Console.WriteLine("\t (Order may change)");
		Console.WriteLine();
		Console.WriteLine("-r  Rules are in the format rule; rule; rule; ...");
		Console.WriteLine("\tEach rule can be written as: original : replacement");
		Console.WriteLine("\tA line can be written as ND, where N represents a number (0-255) for flavor and D a direction, and ... the possibility to include more.");
		Console.WriteLine("\tAvailable directions are U(Up), D(Down), L(Left), R(Right), R(Random, only in replacement), X(any for original, keep direction for replacement), C(Rotate clockwise, only in replacement), W(Rotate counterclockwise, only in replacement), E(Rotate 180 degrees, only in replacement).");
		Console.WriteLine("\tIn the original, several lines can be set to avoid repetition. Separate them with '/'. Example 0D/0U: ...");
		Console.WriteLine("\tIn the replacement, 0 or more lines can be specified, without separators. Example ...: 0L0R0U");
		Console.WriteLine("\tThese rules govern how the linestrip will change over iterations. For example the rule '0D: 0D0D;' will take all lines going down of flavor 0 and replicate them twice.");
		Console.WriteLine("\tWhitespace rules are flexible and it may or may not be present.");
		Console.WriteLine();
		Console.WriteLine("-i  Iteration count, a number");
		Console.WriteLine();
		Console.WriteLine("-s  Starting strip, same format as replacement. For example: 0U0L0D0R for a circle. Naturally only UDLR can be used here.");
		Console.WriteLine();
		Console.WriteLine("-o  Output path. % will be replaced by iteration count.");
		Console.WriteLine();
		Console.WriteLine("-t  Enable text mode: Output will be in text");
		Console.WriteLine();
		Console.WriteLine("-p  Enable open mode: Files will be opened when they are created");
		Console.WriteLine();
		Console.WriteLine("-l  Enable draw only last mode: Only draws last iteration");
		Console.WriteLine();
		Console.WriteLine("-h  Show this help menu");
	}
	
	/* static void Main(string[] args){
		Line[] t = new Line[]{new Line(Dir.U, 0), new Line(Dir.R, 2), new Line(Dir.D, 2)};
		
		//IIterator iter = new CustomIterator("1X:1X;2X:;0U/0D:0C2X2W1X0X2C2E1C0X2C2E1E0W2E2C;0L/0R:0W2X2C1X0X2W2E1W0X2W2E1E0C2E2W;"); //Hilbert
		//IIterator iter = new CustomIterator("2X:0X;0X:0X0C0X"); //J2
		//IIterator iter = new CustomIterator("2X:0x;1X:1X;0X:0X0C0X0W0X0C0X0W0X"); //idk
		IIterator iter = new CustomIterator("0X:2E0W2X1X2X0C2E;1X:3W1X3C;2X:5X;3X:4X;4X:3X3X;5X:3X5X;"); //halffern
		IIterator iter = new CustomIterator("6X:0W6X0C;0X:2X6X2X;1X:2X1X;2X:1X;"); //fern
		//IIterator iter = new FlowerCurve(); //FlowerCurve
		
		while(true){
			Console.WriteLine("Writing");
			Drawer d = new ImageDrawer(t);
			
			d.draw();
			
			ProcessStartInfo psi = new ProcessStartInfo{
				FileName = "art.png",
				UseShellExecute = true
			};
			Process.Start(psi);
			
			t = iter.iterate(t);
			
			Console.ReadKey(true);
		}
	}
	 */
	static Line[] parseSeq(string s){
		int i = 0;
		
		List<Line> l = new();
		
		while(i < s.Length){
			byte f = parseNum(s, ref i);
			Dir d = parseDir(s, ref i);
			l.Add(new Line(d, f));
		}
		
		return l.ToArray();
	}
	
	static byte parseNum(string s, ref int i){
		if(i >= s.Length || !char.IsDigit(s[i])){
			throw new Exception("Expected a number: " + SubAround(s, i));
		}
		
		StringBuilder sb = new();
		
		while(i < s.Length){
			if(!char.IsDigit(s[i])){
				break;
			}
			sb.Append(s[i]);
			
			i++;
		}
		
		if(byte.TryParse(sb.ToString(), out byte parsed)){
			return parsed;
		}
		
		throw new Exception("Invalid number: " + sb.ToString());
	}
	
	static Dir parseDir(string s, ref int i){
		if(i >= s.Length){
			throw new Exception("Expected a direction: " + SubAround(s, i));
		}
		
		i++;
		
		return char.ToUpper(s[i - 1]) switch {
			'U' => Dir.U,
			'D' => Dir.D,
			'L' => Dir.L,
			'R' => Dir.R,
			_ => throw new Exception("Unknown direction: " + SubAround(s, i - 1))
		};
	}
	
	static string SubAround(string s, int index)
	{
		if (string.IsNullOrEmpty(s))
			return string.Empty;
	
		// Clamp index inside the string
		index = Math.Max(0, Math.Min(index, s.Length - 1));
	
		// Try to take one char before and after
		int start = Math.Max(0, index - 1);
		int length = Math.Min(3, s.Length - start);
	
		return "'" + s.Substring(start, length) + "'";
	}
}