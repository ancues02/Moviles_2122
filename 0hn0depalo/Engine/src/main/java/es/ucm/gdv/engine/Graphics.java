package es.ucm.gdv.engine;

/**
 * Interfaz que representa el motor grafico
 * y permite abstraer la implementacion de la plataforma
 */
public interface Graphics {
    /**
     * Devuelve el ancho de la pantalla
     */
    float getWidth();

    /**
     * Devuelve el alto de la pantalla
     */
    float getHeight();

    /**
     * Devuelve el ancho del canvas virtual
     */
    float getCanvasWidth();

    /**
     * Devuelve el alto del canvas virtual
     */
    float getCanvasHeight();

    /**
     * Devuelve el aspect ratio
     */
    float getAspectRatio();

    /**
     * Borra lo que haya en pantalla.
     */
    void clear();

    /**
     * Establece el color con el que se hace el clear.
     *
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    void setBackground(int r, int g, int b, int a);

    /**
     * Mueve el canvas virtual
     *
     * @param x Movimiento en el eje x
     * @param y Movimiento en el eje y
     */
    void translate(float x, int y);

    /**
     * Escala el canvas virtual
     * @param x Escala en el eje x
     * @param y Escala en el eje y
     */
    void scale(float x, float y);

    /**
     * Guarda el estado del canvas virtual
     */
    void save();

    /**
     * Recupera el estado del canvas virtual
     */
    void restore();

    /**
     * Metodo factoria que devuelve una imagen
     * segun el nombre del archivo que la contiene
     *
     * @param name Nombre del archivo
     * @return La imagen creada o null si ha ocurrido algun fallo
     */
    Image newImage(String name);

    /**
     * Dibuja una imagen
     *
     * @param image La imagen que se pintara. Si es null, no se hace nada.
     */
    void drawImage(Image image);

    /**
     * Dibuja una imagen
     *
     * @param image La imagen que se pintara. Si es null, no se hace nada.
     * @param scaleX Escala en x de la imagen
     * @param scaleY Escala en y de la imagen
     */
    void drawImage(Image image, float scaleX, float scaleY);


    /**
     * Dibuja una imagen
     *
     * @param image La imagen que se pintara. Si es null, no se hace nada.
     * @param scaleX Escala en x de la imagen
     * @param scaleY Escala en y de la imagen
     * @param posX Posicion en x de la imagen
     * @param posX Posicion en x de la imagen
     */
    void drawImage(Image image, float scaleX, float scaleY, float posX, float posY);

    /**
     * Dibuja una imagen
     *
     * @param image La imagen que se pintara. Si es null, no se hace nada.
     * @param sizeX Ancho de la imagen en pantalla
     * @param sizeY Alto de la imagen en pantalla
     * @param posX Posicion en x de la imagen
     * @param posX Posicion en x de la imagen
     */
    void drawImage(Image image, int sizeX, int sizeY, float posX, float posY);

    /**
     * Metodo factoria que devuelve una fuente
     * segun el nombre del archivo que la contiene.
     *
     * @param filename Nombre del archivo
     * @param size Tamaño de la fuente
     * @param isBold Fuente en negrita
     * @return La fuente creada o null si ha ocurrido algun fallo
     */
    Font newFont(String filename, float size, boolean isBold);

    /**
     * Establece la fuente para
     * las operaciones de dibujado de textos
     *
     * @param font La fuente que se quiere establecer. Si es null, no se hace nada.
     */
    void setFont(Font font);

    /**
     * Escribe un texto en pantalla segun
     * el font que este establecido
     *
     * @param text Texto que se escribira
     * @param x Coordenada x donde se escribira
     * @param y Coordenada y donde se escribira
     */
    void drawText(String text, float x, float y);

    /**
     * Establece el color con el que
     * se relizaran las operaciones de pintado
     *
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    void setColor(int r, int g, int b, int a);

    //TODO: ¿Usamos este metodo?
    /**
     * Dibuja una circunferencia en pantalla
     *
     * @param cx Coordenada x del centro de la circunferencia
     * @param cy Coordenada x del centro de la circunferencia
     * @param radius Radio de la circunferencia
     */
    void drawCircle(float cx, float cy, float radius);

    /**
     * Dibuja un circulo en pantalla
     *
     * @param cx Coordenada x del centro de la circulo
     * @param cy Coordenada x del centro de la circulo
     * @param radius Radio de la circulo
     */
    void fillCircle(float cx, float cy, float radius);
}