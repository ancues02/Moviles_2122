package es.ucm.gdv.engine;

/**
 * Interfaz que representa las imagenes
 * y permite abstraer la implementacion de la plataforma
 */
public interface Image {
    /**
     * Devuelve el nombre del archivo
     * que contiene la imagen
     *
     * @return el nombre del archivo
     */
    String getName();

    /**
     * Devuelve la anchura de la imagen
     *
     * @return la anchura
     */
    float getWidth();

    /**
     * Devuelve la altura de la imagen
     *
     * @return la altura
     */
    float getHeight();

    /**
     * Devuelve la altura de la imagen
     * ajustada al canvas virtual
     *
     * @return la altura ajustada
     */
    float getHeightInCanvas();

    /**
     * Devuelve la anchura de la imagen
     * ajustada al canvas virtual
     *
     * @return la anchura ajustada
     */
    float getWidthInCanvas();
}
