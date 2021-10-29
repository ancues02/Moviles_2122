package es.ucm.gdv.ohno;

public enum Dirs {
    UP(-1, 0),
    DOWN(1, 0),
    LEFT(0, -1),
    RIGHT(0, 1);

    private final int row, col;

    Dirs(int f, int c){
        this.row = f;
        this.col = c;
    }

    public int getRow(){
        return row;
    }

    public int getCol(){
        return col;
    }
}

