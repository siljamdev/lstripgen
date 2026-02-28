struct Line{
	public Dir dir;
	public byte flavor;
	
	public Line(Dir d, byte f = 0){
		dir = d;
		flavor = f;
	}
	
	public static implicit operator Dir(Line c){
		return c.dir;
	}
	
	public static implicit operator Line(Dir c){
		return new Line(c);
	}
	
	public override string ToString(){
		return flavor.ToString() + dir;
	}
}

enum Dir{
	U, D, L, R
}
