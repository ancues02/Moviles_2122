package es.ucm.gdv.engine;

public interface Graphics {
    float getWidth();
    float getHeight();
    void clear(int r, int g, int b, int a);

    void translate(float x, int y);
    void scale(float x, float y);
    void save();
    void restore();

    Image newImage(String name);
    void drawImage(Image image);
    void drawImage(Image image, float scaleX, float scaleY);
    void drawImage(Image image, float scaleX, float scaleY, float transX, float transY);
    void drawImage(Image image, int sizeX, int sizeY, float transX, float transY);

    Font newFont(String filename, int size, boolean isBold);
    void setFont(Font font);
    void drawText(String text, int x, int y);

    void setColor(int r, int g, int b, int a);
    void fillCircle(float cx, float cy, float radius);
}
