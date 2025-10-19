using System.Text;

class CustomIterator : IIterator{
	Dictionary<Node, Node[]> tree = new();
	
	Random rand = new();
	
	string src;
	int i;
	
	bool atEnd => i >= src.Length;
	
	public CustomIterator(string rules){
		src = rules;
		i = 0;
		
		while(!atEnd){
			(Node, Node[])[] pf = parseEntry();
			
			foreach((Node r, Node[] r2) in pf){
				tryInsertRule(r, r2);
			}
		}
	}
	
	void tryInsertRule(Node og, Node[] rep){
		if(tree.Any(kvp => kvp.Key.flavor == og.flavor && (og.dir == DirExt.X || kvp.Key.dir == DirExt.X || kvp.Key.dir == og.dir))){
			throw new Exception("Repated rule: " + og);
		}
		
		tree.Add(og, rep);
	}
	
	(Node, Node[])[] parseEntry(){
		List<Node> head = new();
		
		parseWhite();
		
		do{
			if(src[i] == '/'){
				i++;
			}
			
			head.Add(parseH());
		}while(!atEnd && src[i] == '/');
		
		parseWhite();
		
		if(atEnd || src[i] != ':'){
			throw new Exception("Expected ':' after rule entry: " + SubAround(src, i));
		}
		i++;
		
		List<Node> res = new();
		
		while(!atEnd){
			parseWhite();
			
			if(src[i] == ';'){
				i++;
				break;
			}
			
			byte f2 = parseNum();
			
			DirExt bd = parseDirB();
			
			res.Add(new Node(bd, f2));
		}
		
		return head.Select(h => (h, res.ToArray())).ToArray();
	}
	
	Node parseH(){
		parseWhite();
		
		byte f = parseNum();
		
		DirExt d = parseDirH();
		
		return new Node(d, f);
	}
	
	DirExt parseDirH(){
		if(atEnd){
			throw new Exception("Expected a direction: " + SubAround(src, i));
		}
		
		i++;
		
		return char.ToUpper(src[i - 1]) switch {
			'U' => DirExt.U,
			'D' => DirExt.D,
			'L' => DirExt.L,
			'R' => DirExt.R,
			'X' => DirExt.X,
			_ => throw new Exception("Unknown direction: " + SubAround(src, i - 1))
		};
	}
	
	DirExt parseDirB(){
		if(atEnd){
			throw new Exception("Expected a direction:" + SubAround(src, i));
		}
		
		i++;
		
		return char.ToUpper(src[i - 1]) switch {
			'U' => DirExt.U,
			'D' => DirExt.D,
			'L' => DirExt.L,
			'R' => DirExt.R,
			'X' => DirExt.X,
			'A' => DirExt.A,
			'C' => DirExt.CW,
			'W' => DirExt.CCW,
			'E' => DirExt.COM,
			_ => throw new Exception("Unknown direction: " + SubAround(src, i - 1))
		};
	}
	
	byte parseNum(){
		if(atEnd || !char.IsDigit(src[i])){
			throw new Exception("Expected a number: " + SubAround(src, i));
		}
		
		StringBuilder sb = new();
		
		while(!atEnd){
			if(!char.IsDigit(src[i])){
				break;
			}
			sb.Append(src[i]);
			
			i++;
		}
		
		if(byte.TryParse(sb.ToString(), out byte parsed)){
			return parsed;
		}
		
		throw new Exception("Invalid number: " + sb.ToString());
	}
	
	void parseWhite(){
		while(!atEnd && char.IsWhiteSpace(src[i])){
			i++;
		}
	}
	
    public Line[] iterate(Line[] d){
        List<Line> r = new(d.Length);
		
		foreach(Line l in d){
			if(tree.Any(kvp => kvp.Key.flavor == l.flavor && (kvp.Key.dir == DirExt.X || kvp.Key.dir == (DirExt) l.dir))){
				KeyValuePair<Node, Node[]> res = tree.First(kvp => kvp.Key.flavor == l.flavor && (kvp.Key.dir == DirExt.X || kvp.Key.dir == (DirExt) l.dir));
				
				foreach(Node od in res.Value){
					r.Add(new Line(getDir(l, od.dir), od.flavor));
				}
			}else{
				r.Add(l);
			}
		}
		
		return r.ToArray();
    }
	
	Dir getDir(Line l, DirExt d){
		return d switch{
			DirExt.U => Dir.U,
			DirExt.D => Dir.D,
			DirExt.L => Dir.L,
			DirExt.R => Dir.R,
			DirExt.A => randDir(),
			DirExt.X => l.dir,
			DirExt.CW => l.dir switch {
				Dir.U => Dir.R,
				Dir.R => Dir.D,
				Dir.D => Dir.L,
				Dir.L => Dir.U,
			},
			DirExt.CCW => l.dir switch {
				Dir.U => Dir.L,
				Dir.R => Dir.U,
				Dir.D => Dir.R,
				Dir.L => Dir.D,
			},
			DirExt.COM => l.dir switch {
				Dir.U => Dir.D,
				Dir.R => Dir.L,
				Dir.D => Dir.U,
				Dir.L => Dir.R,
			},
		};
	}
	
	Dir randDir(){
		return rand.Next(4) switch{
			0 => Dir.U,
			1 => Dir.D,
			2 => Dir.L,
			3 => Dir.R
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

record Node(DirExt dir, byte flavor){
	public override string ToString(){
		return flavor.ToString() + dir;
	}
}

enum DirExt{
	U, D, L, R, X, A, CW, CCW, COM
}