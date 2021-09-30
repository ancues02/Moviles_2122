package es.ucm.gdv.a0hn0depalo;

public interface Font {
    String _filename = null;
    float _size = 0;
    boolean _isBold = false;

    String get_filename();
    boolean IsBold();
    float get_size();

    //filename, size, isBold
}
