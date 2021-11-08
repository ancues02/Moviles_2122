package es.ucm.gdv.engine;

public interface Graphics {
    float getWidth();
    float getHeight();
    float getCanvasWidth();
    float getCanvasHeight();
    void clear(int r, int g, int b, int a);

    void translate(float x, int y);
    void scale(float x, float y);
    void save();
    void restore();

    Image newImage(String name);
    void drawImage(Image image);
    void drawImage(Image image, float scaleX, float scaleY);
    void drawImage(Image image, float scaleX, float scaleY, float posX, float posY);
    void drawImage(Image image, int sizeX, int sizeY, float posX, float posY);

    Font newFont(String filename, float size, boolean isBold);
    void setFont(Font font);
    void drawText(String text, float x, float y);

    void setColor(int r, int g, int b, int a);
    void fillCircle(float cx, float cy, float radius);
}