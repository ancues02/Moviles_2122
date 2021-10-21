package es.ucm.gdv.launcher.android;

public interface Graphics {
    float _windowX = 0, _windowY = 0;

    Image newImage(String name);
    Font newFont(String filename, float size, boolean isBold);
    void clear(int r, int g, int b, int a);

    void translate(float x, int y);
    void scale(float x, float y);
    void save();
    void restore();

    void drawImage(Image image);
    void drawImage(Image image, float scaleX, float scaleY);
    void drawImage(Image image, float scaleX, float scaleY, float transX, float transY);

    void setColor(int r, int g, int b, int a);

    void fillCircle(float cx, float cy, float radius);

    void drawText(String text, float x, float y);

    float get_windowX();

    float get_windowY();
}
